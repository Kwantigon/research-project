using Backend.Abstractions;

namespace Backend.RequestHandlers;

public class GetRequestsHandler(
	ILogger<GetRequestsHandler> logger,
	IDatabase database,
	IPromptConstructor promptConstructor,
	ILlmConnector llmConnector,
	ILlmResponseProcessor llmResponseProcessor
) : IGetRequestsHandler
{
	private readonly ILogger<GetRequestsHandler> _logger = logger;

	private readonly IDatabase _database = database;

	private readonly IPromptConstructor _promptConstructor = promptConstructor;

	private readonly ILlmConnector _llmConnector = llmConnector;

	private readonly ILlmResponseProcessor _llmResponseProcessor = llmResponseProcessor;

	#region Interface implementation
	public DataSpecificationListDTO GetAllDataSpecifications()
	{
		var dataSpecifications = database.GetAllDataSpecifications();
		var dataSpecificationsDTOs = dataSpecifications.Select(
			dataSpec => new DataSpecificationListDTO()
			{
				Name = dataSpec.Name,
				Location = "/data-specifications/" + dataSpec.Id,
				DataspecerIri = dataSpec.DataspecerIri,
			}
		).ToList();
		return new DataSpecificationListDTO { DataSpecifications = dataSpecificationsDTOs };
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

	public ConversationListDTO GetAllConversations()
	{
		var conversations = database.GetAllConversations();
		var conversationDTOs = conversations.Select(
			conversation => new ConversationDTO()
			{
				Title = conversation.Title,
				Location = "/conversations/" + conversation.Id
			}
		).ToList();
		return new ConversationListDTO { Conversations = conversationDTOs };
	}

	public ConversationDTO GetConversation(uint conversationId)
	{
		var conversation = _database.GetConversationById(conversationId);
		return new ConversationDTO
		{
			Title = conversation.Title,
			Location = "/conversations/" + conversation.Id;
		};
	}
	#endregion
}
