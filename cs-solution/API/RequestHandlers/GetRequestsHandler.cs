using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;

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
	#endregion
}
