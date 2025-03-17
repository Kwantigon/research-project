using Backend.Model;

namespace Backend.Abstractions;

public interface IDatabase
{
	IList<DataSpecification> GetAllDataSpecifications();

	DataSpecification? GetDataSpecificationById(uint dataSpecificationId);

	IList<Conversation> GetAllConversations();

	Conversation? GetConversationById(uint conversationId);

	bool AddNewDataSpecification(DataSpecification dataSpecification);

	bool AddNewConversation(Conversation conversation);
}
