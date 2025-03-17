using Backend.Abstractions;
using Backend.Model;
using Backend.Exceptions;

namespace Backend.Implementation.Database;

public class InMemoryDatabase(ILogger<InMemoryDatabase> logger) : IDatabase
{
	private readonly ILogger<InMemoryDatabase> _logger = logger;
	private Dictionary<uint, DataSpecification> _dataSpecifications = new Dictionary<uint, DataSpecification>();
	private Dictionary<uint, Conversation> _conversations = new Dictionary<uint, Conversation>();

	public IList<DataSpecification> GetAllDataSpecifications()
	{
		return _dataSpecifications.Values.ToList();
	}

	public DataSpecification? GetDataSpecificationById(uint dataSpecificationId)
	{
		_dataSpecifications.TryGetValue(dataSpecificationId, out var dataSpecification);
		return dataSpecification;
	}

	public IList<Conversation> GetAllConversations()
	{
		return _conversations.Values.ToList();
	}

	public Conversation? GetConversationById(uint conversationId)
	{
		_conversations.TryGetValue(conversationId, out var conversation);
		return conversation;
	}

	public bool AddNewDataSpecification(DataSpecification dataSpecification)
	{
		if (_dataSpecifications.ContainsKey(dataSpecification.Id))
		{
			_logger.LogError("The database already contains a data specification with ID {DataSpecId}", dataSpecification.Id);
			return false;
		}
		_dataSpecifications.Add(dataSpecification.Id, dataSpecification);
		return true;
	}

	public bool AddNewConversation(Conversation conversation)
	{
		if (_conversations.ContainsKey(conversation.Id))
		{
			_logger.LogError("The database already contains a conversation with ID {ConversationId}", conversation.Id);
			return false;
		}
		_conversations.Add(conversation.Id, conversation);
		return true;
	}
}
