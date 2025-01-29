using BackendApi.Abstractions;
using BackendApi.Model;
using BackendApi.Exceptions;

namespace BackendApi.Database;

public class InMemoryDatabase : IDatabase
{
	public InMemoryDatabase()
	{
		_dataSpecifications = new Dictionary<uint, DataSpecification>();
		_conversations = new Dictionary<uint, Conversation>();
	}

	private Dictionary<uint, DataSpecification> _dataSpecifications;

	private Dictionary<uint, Conversation> _conversations;

	public IList<DataSpecification> GetAllDataSpecifications()
	{
		return _dataSpecifications.Values.ToList();
	}

	public DataSpecification GetDataSpecificationById(uint dataSpecificationId)
	{
		if (_dataSpecifications.TryGetValue(dataSpecificationId, out var dataSpecification))
		{
			return dataSpecification;
		}
		else
		{
			throw new DataException($"Data specification with ID {dataSpecificationId} not found");
		}
	}

	public IList<Conversation> GetAllConversations()
	{
		return _conversations.Values.ToList();
	}

	public Conversation GetConversationById(uint conversationId)
	{
		if (_conversations.TryGetValue(conversationId, out var conversation))
		{
			return conversation;
		}
		else
		{
			throw new DataException($"Data specification with ID {conversationId} not found");
		}
	}

	public void AddNewDataSpecification(DataSpecification dataSpecification)
	{
		_dataSpecifications.Add(dataSpecification.Id, dataSpecification);
	}

	public void AddNewConversation(Conversation conversation)
	{
		_conversations.Add(conversation.Id, conversation);
	}
}
