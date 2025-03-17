using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;

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
			throw new Exception("Could not find the requested data specification");
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
			throw new Exception("Could not find the requested conversation");
		}

		return conversation;
	}

	public List<MessageBasicDTO> GetConversationMessages(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			throw new Exception("Could not find the requested conversation");
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

	public MessageDetailedDTO GetMessageFromConversation(uint conversationId, uint messageId)
	{
		throw new NotImplementedException();
	}

	public DataSpecificationItemSummaryDTO GetItemSummaryFromDataSpecification(uint dataSpecificationId, uint itemId)
	{
		throw new NotImplementedException();
	}

	public NextMessagePreviewDTO GetNextMessagePreview(uint conversationId)
	{
		throw new NotImplementedException();
	}
	#endregion
}
