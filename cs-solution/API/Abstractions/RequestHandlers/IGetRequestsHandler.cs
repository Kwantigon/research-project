using Backend.DTO;

namespace Backend.Abstractions.RequestHandlers;

public interface IGetRequestsHandler
{
	AboutDTO GetAbout();

	List<DataSpecificationDTO> GetAllDataSpecifications();

	DataSpecificationDTO GetDataSpecification(uint dataSpecificationId);

	List<ConversationDTO> GetAllConversations();

	ConversationDTO GetConversation(uint conversationId);

	List<MessageBasicDTO> GetConversationMessages(uint conversationId);

	MessageDetailedDTO GetMessageFromConversation(uint conversationId, uint messageId);

	DataSpecificationItemSummaryDTO GetItemSummaryFromDataSpecification(uint dataSpecificationId, uint itemId);

	NextMessagePreviewDTO GetNextMessagePreview(uint conversationId);
}
