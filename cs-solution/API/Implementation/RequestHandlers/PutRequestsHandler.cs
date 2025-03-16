using Backend.Abstractions.RequestHandlers;
using Backend.DTO;

namespace Backend.Implementation.RequestHandlers;

public class PutRequestsHandler(
	ILogger<PutRequestsHandler> logger) : IPutRequestsHandler
{
	private readonly ILogger<PutRequestsHandler> _logger = logger;

	#region Interface implementation.

	public IResult PutConversationNextMessagePreview(uint conversationId, PutConversationNextMessagePreviewDTO payload)
	{
		throw new NotImplementedException();
	}
	#endregion
}
