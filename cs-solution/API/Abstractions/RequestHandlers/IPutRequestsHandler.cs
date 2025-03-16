using Backend.DTO;

namespace Backend.Abstractions.RequestHandlers;

public interface IPutRequestsHandler
{
	IResult PutConversationNextMessagePreview(uint conversationId, PutConversationNextMessagePreviewDTO payload);
}
