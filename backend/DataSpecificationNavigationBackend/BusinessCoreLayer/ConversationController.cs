using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IConversationService""/> to HTTP responses.
/// </summary>
public class ConversationController(
	ILogger<ConversationController> logger,
	IConversationService conversationService,
	IDataSpecificationService dataSpecificationService,
	AppDbContext appDbContext) : IConversationController
{
	private readonly ILogger<ConversationController> _logger = logger;
	private readonly IConversationService _conversationService = conversationService;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	private readonly AppDbContext _database = appDbContext; // Will be removed later.

	public async Task<IResult> StartConversationAsync(PostConversationsDTO payload)
	{
		DataSpecification? dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecerAsync(payload.DataspecerPackageUuid, payload.DataspecerPackageName);
		if (dataSpecification is null)
		{
			return Results.InternalServerError(new ErrorDTO() { Reason = "There was an error while retrieving and processing the Dataspecer package." });
		}
		Conversation conversation = await _conversationService.StartNewConversationAsync(payload.ConversationTitle, dataSpecification);
		return Results.Created($"/conversations/{conversation.Id}", (ConversationDTO)conversation);
	}

	public async Task<IResult> GetAllConversationsAsync()
	{
		IReadOnlyList<Conversation> conversations = await _conversationService.GetAllConversationsAsync();
		return Results.Ok(
			conversations.Select(conv => (ConversationDTO)conv)
		);
	}

	public async Task<IResult> GetConversationAsync(int conversationId)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation is null)
		{
			return Results.NotFound(new ErrorDTO { Reason = $"Conversation with ID {conversationId} not found." });
		}

		return Results.Ok((ConversationDTO)conversation);
	}

	public async Task<IResult> GetConversationMessagesAsync(int conversationId)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId, includeMessages: true);
		if (conversation == null)
		{
			return Results.NotFound(new ErrorDTO { Reason = $"Conversation with ID {conversationId} not found." });
		}

		return Results.Ok(
			conversation.Messages.Select(msg => (ConversationMessageDTO)msg)
		);
	}

	public async Task<IResult> GetMessageAsync(int conversationId, Guid messageId)
	{
		_logger.LogTrace("Retrieving the conversation with ID {Id}.", conversationId);
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId, includeMessages: true);
		if (conversation is null)
		{
			_logger.LogError("Conversation with Id {Id} not found.", conversationId);
			return Results.NotFound(new ErrorDTO { Reason = $"Conversation with ID {conversationId} not found." });
		}

		_logger.LogTrace("Searching for the message with ID {Id} in the conversation.", messageId);
		Message? requestedMessage = conversation.Messages.Find(msg => msg.Id == messageId);
		if (requestedMessage is null)
		{
			_logger.LogError("Conversation [Title={ConvTitle}, Id={ConvId}] does not contain the message with ID {MsgId}.",
																		conversation.Title, conversationId, messageId);
			return Results.NotFound(new ErrorDTO { Reason = $"Message with ID {messageId} not found." });
		}

		_logger.LogTrace("Found the requested message in conversation [Title={ConvTitle}, Id={ConvId}]", conversation.Title, conversation.Id);
		if (requestedMessage.Type is MessageType.ReplyMessage && requestedMessage.TextValue == string.Empty)
		{
			_logger.LogTrace("The reply message does not contain any text. That means it has not yet been generated.");

			_logger.LogTrace("Searching for the user message preceding this reply message.");
			Message? userMessage = conversation.Messages.Find(msg => msg.ReplyMessageId == messageId);
			if (userMessage is null)
			{
				_logger.LogError("The user message preceding reply message with ID={ReplyId} was not found in the conversation.", requestedMessage.Id);
				return Results.NotFound(new ErrorDTO { Reason = $"User message preceding the message with ID {messageId} not found." });
			}

			// Todo: Guard this critical section with a semaphore.
			_logger.LogTrace("Critical section start: _conversationService.GenerateReplyMessage(userMessage)");
			Message? reply = await _conversationService.GenerateReplyMessage(userMessage);
			_logger.LogTrace("Critical section end: _conversationService.GenerateReplyMessage(userMessage)");
			// End of critical section.
			if (reply is null)
			{
				return Results.InternalServerError(new ErrorDTO() { Reason = "An error occured while generating a reply." });
			}

			return Results.Ok((ConversationMessageDTO)reply);
		}
		else
		{
			return Results.Ok((ConversationMessageDTO)requestedMessage);
		}
	}

	public async Task<IResult> ProcessUserMessageAsync(int conversationId, PostConversationMessagesDTO payload)
	{
		_logger.LogInformation("Processing incoming user message. ConversationId = {ConvId}, payload = {Payload}", conversationId, payload);

		_logger.LogTrace("Searching for the conversation.");
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation == null)
		{
			_logger.LogError("Conversation with ID {ConvId} not found.", conversationId);
			return Results.NotFound(new ErrorDTO { Reason = "Conversation not found." });
		}

		_logger.LogTrace("Adding the user message to the conversation.");
		Message userMessage = await _conversationService.AddNewUserMessage(conversation, payload.TextValue, DateTime.Now, payload.UserModifiedSuggestedMessage);

		return Results.Created($"/conversations/{conversationId}/messages/{userMessage.Id}", (ConversationMessageDTO)userMessage);
	}

	public async Task<IResult> AddSelectedItemsAndGetSuggestedMessage(int conversationId, PutDataSpecItemsDTO payload)
	{
		_logger.LogTrace("Items selected by the user for conversation with ID {Id}: {ItemIris}", conversationId, payload.ItemIriList);

		_logger.LogTrace("Searching for the conversation with ID={Id}", conversationId);
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId, includeMessages: true);
		if (conversation is null)
		{
			_logger.LogError("Conversation with ID={Id} not found.", conversationId);
			return Results.NotFound(new ErrorDTO() { Reason = "Conversation not found" });
		}

		_logger.LogTrace("Searching for the selected items.");
		List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecificationId, payload.ItemIriList);
		if (selectedItems.Count != payload.ItemIriList.Count)
		{
			List<string> itemsNotFound = payload.ItemIriList.Where(iri => !selectedItems.Any(itemFound => itemFound.Iri == iri)).ToList();
			_logger.LogError("The following item IRIs were not found: {Items}", itemsNotFound);
			return Results.BadRequest(new ErrorDTO() { Reason = "One or more selected items are not present in the data specification." });
		}

		_logger.LogTrace("Generating a suggested message.");
		string? suggestedMessage = await _conversationService.GenerateSuggestedMessageAsync(conversation, selectedItems);
		if (string.IsNullOrEmpty(suggestedMessage))
		{
			_logger.LogError("The suggested message is either null or empty.");
			return Results.InternalServerError(new ErrorDTO() { Reason = "There was an error while generating the suggested message." });
		}

		_logger.LogTrace("Returning the suggested message: {SuggestedMessage}", suggestedMessage);
		return Results.Ok(new SuggestedMessageDTO(suggestedMessage));
	}

	// Todo: Should call a method on ConversationService.
	// That method shoudld remove all other things related to the conversation.
	public async Task<IResult> DeleteConversationAsync(int conversationId)
	{
		Conversation? conversation = await _database.Conversations.SingleOrDefaultAsync(c => c.Id == conversationId);
		if (conversation is null)
		{
			return Results.NotFound($"Conversation with ID {conversationId} not found.");
		}

		DataSpecification dataSpecification = conversation.DataSpecification;
		_database.DataSpecifications.Remove(dataSpecification);
		_database.Conversations.Remove(conversation);
		_database.SaveChanges();
		return Results.NoContent();
	}
}
