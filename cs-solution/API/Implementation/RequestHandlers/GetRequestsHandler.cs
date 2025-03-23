using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;

namespace Backend.Implementation.RequestHandlers;

public class GetRequestsHandler(ILogger<GetRequestsHandler> logger, IDatabase database)
	: IGetRequestsHandler
{
	private readonly ILogger<GetRequestsHandler> _logger = logger;

	private readonly IDatabase _database = database;

	#region Interface implementation

	public AboutDTO GetAbout()
	{
		return new AboutDTO
		{
			ServiceName = "Back end API",
			Greeting = "Hello there!"
		};
	}

	public List<DataSpecificationDTO> GetAllDataSpecifications()
	{
		var dataSpecifications = _database.GetAllDataSpecifications();
		return dataSpecifications.Select(
			dataSpec => new DataSpecificationDTO()
			{
				Name = dataSpec.Name,
				Location = "/data-specifications/" + dataSpec.Id,
				DataspecerIri = dataSpec.DataspecerIri,
			}
		).ToList();
	}

	public DataSpecificationDTO GetDataSpecification(uint dataSpecificationId)
	{
		var dataSpecification = _database.GetDataSpecificationById(dataSpecificationId);
		if (dataSpecification is null)
		{
			_logger.LogError("Failed to retrieve the data specification with ID {DataSpecId} from the database.", dataSpecificationId);
			throw new Exception("Failed to find the requested data specification");
		}

		return new DataSpecificationDTO
		{
			Name = dataSpecification.Name,
			Location = "/data-specification/" + dataSpecification.Id,
			DataspecerIri = dataSpecification.DataspecerIri
		};
	}

	public List<ConversationDTO> GetAllConversations()
	{
		var conversations = _database.GetAllConversations();
		return conversations.Select(
			conversation => (ConversationDTO)conversation
		).ToList();
	}

	public ConversationDTO GetConversation(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			throw new Exception("Failed to find the requested conversation");
		}

		return conversation;
	}

	public List<MessageBasicDTO> GetConversationMessages(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			throw new Exception("Failed to find the requested conversation");
		}

		return conversation.Messages.Select(message =>
		{
			MessageSource source;
			if (message is UserMessage)
			{
				source = MessageSource.User;
			}
			else
			{
				source = MessageSource.System;
			}

			return new MessageBasicDTO
			{
				Source = source,
				TimeStamp = message.TimeStamp,
				Text = message.TextValue
			};
		}).ToList();
	}

	public IResult GetMessageFromConversation(uint conversationId, uint messageId)
	{
		Conversation? conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Conversation with ID {ConversationId} not found.", conversationId);
			return Results.NotFound(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		Message? message = conversation.Messages.Find(msg =>  msg.Id == messageId);
		if (message is null)
		{
			_logger.LogError("Conversation with ID {ConversationId} does not contain the message with ID {MessageId}.", conversationId, messageId);
			return Results.NotFound(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the message with ID {messageId}"
			});
		}

		var messageDTO = new MessageDetailedDTO()
		{
			Id = message.Id,
			TimeStamp = message.TimeStamp,
			TextValue = message.TextValue
		};
		switch (message)
		{
			case UserMessage userMessage:
				messageDTO.Type = MessageType.UserMessage;
				if (userMessage.SystemAnswer is not null)
				{
					messageDTO.SystemAnswerIri = $"/conversations/{conversation.Id}/messages/{userMessage.SystemAnswer.Id}";
				}
				break;
			case PositiveSystemAnswer positiveSystemAnswer:
				messageDTO.MatchedDataSpecificationItems = positiveSystemAnswer.MatchedItems;
				break;
			default:
				break;
		}

		return Results.Ok(messageDTO);
	}

	public IResult GetItemSummaryFromDataSpecification(uint dataSpecificationId, uint itemId)
	{
		/*DataSpecification? dataSpecification = _database.GetDataSpecificationById(dataSpecificationId);
		if (dataSpecification is null)
		{
			_logger.LogError("Failed to retrieve the data specification with ID {DataSpecId} from the database.", dataSpecificationId);
			return Results.NotFound(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = "Failed to find the data specification"
			});
		}*/

		if (_database.DataSpecificationExists(dataSpecificationId) is false)
		{
			_logger.LogError("The data specification with ID {DataSpecId} does not exist in the database.", dataSpecificationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the data specification with ID {dataSpecificationId}"
			});
		}

		DataSpecificationItem? dataSpecificationItem = _database.GetDataSpecificationItem(dataSpecificationId, itemId);
		if (dataSpecificationItem is null)
		{
			_logger.LogError("Failed to retrieve the data specification item with ID {ItemId} from the database.", itemId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the data specification item with ID {itemId}"
			});
		}

		string itemSummaryPrompt = _promptConstructor.CreateItemSummaryPrompt(dataSpecificationItem);
		string itemSummaryResponse = _llmConnector.SendPrompt(itemSummaryPrompt);
		DataSpecificationItemSummary summary = _llmResponseProcessor.ProcessSummaryResponse(itemSummaryResponse);

		return Results.Ok(new DataSpecificationItemSummaryDTO
		{
			TextualSummary = summary.Text,
			IriOfRelatedItems = summary.RelatedItems.Select(item => $"/data-specifications/{dataSpecificationId}/{item.Id}").ToList()
		});
	}

	public NextMessagePreviewDTO GetNextMessagePreview(uint conversationId)
	{
		throw new NotImplementedException();
	}
	#endregion
}
