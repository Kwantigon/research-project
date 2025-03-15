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
			conversation => new ConversationDTO()
			{
				Title = conversation.Title,
				Location = "/conversations/" + conversation.Id
			}
		).ToList();
	}

	public ConversationDTO GetConversation(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		return new ConversationDTO
		{
			Title = conversation.Title,
			Location = "/conversations/" + conversation.Id
		};
	}

	public List<MessageBasicDTO> GetConversationMessages(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		return conversation.Messages.Select(message =>
		{
			switch (message)
			{
				case UserMessage userMsg:
					return new MessageBasicDTO
					{
						Source = MessageSource.User,
						TimeStamp = userMsg.TimeStamp,
						Text = userMsg.TextValue
					};
				default:
					throw new NotSupportedException("Unexpected message class: " + message.GetType().Name);
			}
		}).ToList();
	}

	public MessageDTO GetMessageFromConversation(uint conversationId, uint messageId)
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
