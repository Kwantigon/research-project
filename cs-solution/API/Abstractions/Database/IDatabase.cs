using Backend.Model;

namespace Backend.Abstractions.Database;

public interface IDatabase
{
	IList<DataSpecification> GetAllDataSpecifications();

	DataSpecification? GetDataSpecificationById(uint dataSpecificationId);

	IList<Conversation> GetAllConversations();

	Conversation? GetConversationById(uint conversationId);

	DataSpecificationItem? GetDataSpecificationItem(uint dataSpecificationId, uint itemId);

	List<DataSpecificationItem> GetDataSpecificationItems(uint dataSpecificationId, IReadOnlyList<uint> dataSpecificationItemIdList);

	bool AddNewDataSpecification(DataSpecification dataSpecification);

	bool AddNewConversation(Conversation conversation);

	bool DataSpecificationExists(uint dataSpecificationId);

	bool DataSpecificationItemExists(uint dataSpecificationId, uint itemId);
}
