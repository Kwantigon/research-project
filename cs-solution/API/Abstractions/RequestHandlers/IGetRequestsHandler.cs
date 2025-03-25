using Backend.DTO;
using Microsoft.AspNetCore.Http;

namespace Backend.Abstractions.RequestHandlers;

public interface IGetRequestsHandler
{
	AboutDTO GetAbout();

	List<DataSpecificationDTO> GetAllDataSpecifications();

	DataSpecificationDTO GetDataSpecification(uint dataSpecificationId);

	List<ConversationDTO> GetAllConversations();

	ConversationDTO GetConversation(uint conversationId);

	IResult GetConversationMessages(uint conversationId);

	IResult GetMessageFromConversation(uint conversationId, uint messageId);

	IResult GetItemSummaryFromDataSpecification(uint dataSpecificationId, uint itemId);

	IResult GetNextMessagePreview(uint conversationId);
}
