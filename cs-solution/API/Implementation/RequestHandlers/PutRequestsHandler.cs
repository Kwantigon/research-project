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
	ILlmResponseProcessor llmResponseProcessor,
	IConversationService conversationService) : IPutRequestsHandler
{
	private readonly ILogger<PutRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;
	private readonly IConversationService _conversationService = conversationService;
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

		List<DataSpecificationItem> userSelectedItems = _database.GetDataSpecificationItems(conversation.DataSpecification.Id, payload.SelectedItemsIds);
		List<uint> itemIdsNotFound = userSelectedItems
			.Select(item => item.Id)
			.Except(payload.SelectedItemsIds)
			.ToList();
		if (itemIdsNotFound.Any())
		{
			_logger.LogError("Some data specification items were not found in the database: {NotFoundItemsIds}", itemIdsNotFound);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = "Failed to find one or more data specification items"
			});
		}

		_conversationService.AddItemsToSubstructurePreview(conversation, userSelectedItems);
		// conversation.NextQuestionSubstructurePreview should now be initialized and assigned to after the previous call.
		string questionPreviewPrompt = _promptConstructor.CreateQuestionPreviewPrompt(conversation.NextQuestionSubstructurePreview!);

		string questionPreviewResponse = _llmConnector.SendPromptAndReceiveResponse(questionPreviewPrompt);
		string questionPreview = _llmResponseProcessor.ProcessQuestionPreviewResponse(questionPreviewResponse);
		_conversationService.UpdateQuestionPreview(conversation, questionPreview);

		return Results.Created();
	}
	#endregion
}
