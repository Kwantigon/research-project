using Backend.Abstractions.Database;
using Backend.Abstractions.LlmServices;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;

namespace Backend.Implementation.RequestHandlers;

public class GetRequestsHandler(
	ILogger<GetRequestsHandler> logger,
	IDatabase database,
	IPromptConstructor promptConstructor,
	ILlmConnector llmConnector,
	ILlmResponseProcessor llmResponseProcessor) : IGetRequestsHandler
{
	private readonly ILogger<GetRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;
	private readonly IPromptConstructor _promptConstructor = promptConstructor;
	private readonly ILlmConnector _llmConnector = llmConnector;
	private readonly ILlmResponseProcessor _llmResponseProcessor = llmResponseProcessor;

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

	public IResult GetConversationMessages(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = "Failed to find the requested conversation"
			});
		}

		var messagesList = conversation.Messages.Select(message =>
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

		return Results.Ok(messagesList);
	}

	public IResult GetMessageFromConversation(uint conversationId, uint messageId)
	{
		Conversation? conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Conversation with ID {ConversationId} not found.", conversationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		Message? message = conversation.Messages.Find(msg =>  msg.Id == messageId);
		if (message is null)
		{
			_logger.LogError("Conversation with ID {ConversationId} does not contain the message with ID {MessageId}.", conversationId, messageId);
			return Results.NotFound(new ErrorResponseDTO
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
				messageDTO.MatchedDataSpecificationItems = positiveSystemAnswer.MatchedItems
					.Select(item => new DataSpecificationItemDTO
					{
						Name = item.Name,
						Location = $"/data-specifications/{conversation.DataSpecification.Id}/items/{item.Id}"
					}).ToList();
				break;
			default:
				break;
		}

		return Results.Ok(messageDTO);
	}

	public IResult GetItemSummaryFromDataSpecification(uint dataSpecificationId, uint itemId)
	{
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
				ErrorMessage = "Failed to find the data specification item"
			});
		}

		if (dataSpecificationItem.Summary is null)
		{
			string itemSummaryPrompt = _promptConstructor.CreateItemSummaryPrompt(dataSpecificationItem);
			string itemSummaryResponse = _llmConnector.SendPromptAndReceiveResponse(itemSummaryPrompt);
			DataSpecificationItemSummary summary = _llmResponseProcessor.ProcessItemsSummaryResponse(itemSummaryResponse);
			dataSpecificationItem.Summary = summary;
		}
		

		return Results.Ok(new DataSpecificationItemSummaryDTO
		{
			TextualSummary = dataSpecificationItem.Summary.TextualSummary,
			IriOfRelatedItems = dataSpecificationItem.Summary.RelatedItemsIds.Select(relatedItemId => $"/data-specifications/{dataSpecificationId}/items/{relatedItemId}/summary").ToList()
		});
	}

	public IResult GetNextMessagePreview(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		if (conversation.NextQuestionPreview is null)
		{
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = "The preview for the next question has not been generated yet."
			});
		}
		
		return Results.Ok(new NextMessagePreviewDTO { TextualValue = conversation.NextQuestionPreview.TextValue });
	}
	#endregion
}
