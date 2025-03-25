using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;

namespace Backend.Implementation.RequestHandlers;

public class PutRequestsHandler(
	ILogger<PutRequestsHandler> logger,
	IDatabase database,
	IPromptConstructor promptConstructor,
	ILlmConnector llmConnector,
	ILlmResponseProcessor llmResponseProcessor) : IPutRequestsHandler
{
	private readonly ILogger<PutRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;
	private readonly IPromptConstructor _promptConstructor = promptConstructor;
	private readonly ILlmConnector _llmConnector = llmConnector;
	private readonly ILlmResponseProcessor _llmResponseProcessor = llmResponseProcessor;

	#region Interface implementation.

	public IResult PutConversationNextMessagePreview(uint conversationId, PutConversationNextMessagePreviewDTO payload)
	{
		if (payload.SelectedItemsIds is null || payload.SelectedItemsIds.Count == 0)
		{
			_logger.LogError("{PayloadSelectedItems} is either null or empty.", nameof(payload.SelectedItemsIds));
			return Results.BadRequest(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "Payload does not contain any item IDs"
			});
		}

		Conversation? conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		foreach (uint itemId in payload.SelectedItemsIds)
		{
			if (_database.DataSpecificationItemExists(itemId) is false)
			{
				_logger.LogError("Item with ID {ItemId} does not exist in the database.", itemId);
				return Results.NotFound(new ErrorResponseDTO
				{
					ErrorCode = HttpStatusCode.NotFound,
					ErrorMessage = $"Failed to find the item with ID {itemId}"
				});
			}
		}
		// Add all items at once instead of one by one in the loop above.
		// Because if I find an invalid item in the loop, I want to return immediately,
		// but then I will have some items added to the substructure already.
		_conversationService.AddItemsToSubstructure(conversation, payload.SelectedItemsIds);

		string questionPreviewPrompt = _promptConstructor.CreateQuestionPreviewPrompt(conversation.SubstructurePreview);
		string questionPreviewResponse = _llmConnector.SendPromptAndReceiveResponse(questionPreviewPrompt);
		string questionPreview = _llmResponseProcessor.ProcessQuestionPreviewResponse(questionPreviewResponse);
		_conversationService.UpdateQuestionPreview(conversation, questionPreview);
	}
	#endregion
}
