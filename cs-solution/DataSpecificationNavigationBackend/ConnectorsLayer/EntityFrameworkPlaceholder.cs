using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.ConnectorsLayer;

public class EntityFrameworkPlaceholder
{
	private readonly List<DataSpecification> _dataSpecifications = [];
	private readonly List<Conversation> _conversations = [];

	public void Save(object entity)
	{
		if (entity == null) return;

		if (entity.GetType() == typeof(Conversation))
		{
			_conversations.Add(entity as Conversation);
		}

		if (entity.GetType() == typeof(DataSpecification))
		{
			_dataSpecifications.Add(entity as DataSpecification);
		}
	}

	public DataSpecification? FindDataSpecificationByIri(string dataSpecificationIri) { return null; }

	public List<Conversation> GetAllConversations() { return _conversations; }

	public Conversation? FindConversationById(int conversationId) { return null; }
}
