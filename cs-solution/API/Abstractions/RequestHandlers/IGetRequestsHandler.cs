using Backend.DTO;

namespace Backend.Abstractions.RequestHandlers;

public interface IGetRequestsHandler
{
	List<DataSpecificationDTO> GetAllDataSpecifications();

	DataSpecificationDTO GetDataSpecification(uint dataSpecificationId);

	List<ConversationDTO> GetAllConversations();

	ConversationDTO GetConversation(uint conversationId);

	List<MessageBasicDTO> GetConversationMessages(uint conversationId);

	MessageDTO GetMessageFromConversation(uint conversationId, uint messageId);
}
