public interface IGetRequestsHandler
{
	DataSpecificationListDTO GetAllDataSpecifications();

	DataSpecificationDTO GetDataSpecification(uint dataSpecificationId);

	ConversationListDTO GetAllConversations();

	ConversationDTO GetConversation(uint conversationId);
}
