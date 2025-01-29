using BackendApi.Model;

namespace BackendApi.Abstractions;

public interface IDatabase
{
	IList<DataSpecification> GetAllDataSpecifications();

	DataSpecification GetDataSpecificationById(uint dataSpecificationId);

	IList<Conversation> GetAllConversations();

	Conversation GetConversationById(uint  conversationId);

	void AddNewDataSpecification(DataSpecification dataSpecification);

	void AddNewConversation(Conversation conversation);
}
