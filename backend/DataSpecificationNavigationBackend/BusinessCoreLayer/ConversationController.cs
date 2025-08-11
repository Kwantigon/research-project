using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO.Transformer;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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
	private readonly AppDbContext _database = appDbContext; // To get the range object of ObjectProperties.
	// To do: Eventually restructure the code so that I can remove the database from this controller.

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

		List<ConversationMessageDTO> responseDTO = [];
		foreach (Message msg in conversation.Messages)
		{
			ConversationMessageDTO messageDTO;

			if (msg is ReplyMessage replyMessage)
			{
				messageDTO = BuildMessageDTOFromReply(replyMessage);
			}
			else
			{
				messageDTO = new()
				{
					Id = msg.Id,
					Sender = msg.Sender,
					TextContent = msg.TextContent,
					Timestamp = msg.Timestamp
				};
				if (msg is UserMessage userMessage)
				{

					messageDTO.ReplyMessageUri = $"/conversations/{conversationId}/messages/{userMessage.ReplyMessageId}";
				}
			}

			responseDTO.Add(messageDTO);
		}

		return Results.Ok(responseDTO);
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
		if (requestedMessage is ReplyMessage replyMessage)
		{
			UserMessage userMessage = replyMessage.PrecedingUserMessage;

			if (replyMessage.IsGenerated is false)
			{
				_logger.LogTrace("The reply message has not yet been generated. Will do so now.");

				// Todo: Guard this critical section with a semaphore.
				_logger.LogTrace("Critical section start: _conversationService.GenerateReplyMessage(userMessage)");
				ReplyMessage? reply = await _conversationService.GenerateReplyMessageAsync(userMessage);
				_logger.LogTrace("Critical section end: _conversationService.GenerateReplyMessage(userMessage)");
				// End of critical section.
				if (reply is null)
				{
					return Results.InternalServerError(new ErrorDTO() { Reason = "An error occured while generating a reply." });
				}
				else
				{
					replyMessage = reply;
				}
			}

			// Build the DTO to return.
			ConversationMessageDTO replyMessageDTO = BuildMessageDTOFromReply(replyMessage);
			
			return Results.Ok(replyMessageDTO);
		}
		else if (requestedMessage is UserMessage userMessage)
		{
			return Results.Ok(new ConversationMessageDTO
			{
				Id = userMessage.Id,
				Sender = userMessage.Sender,
				TextContent = userMessage.TextContent,
				Timestamp = userMessage.Timestamp,
				ReplyMessageUri = $"/conversations/{conversation.Id}/messages/{userMessage.ReplyMessageId}"
			});
		}
		else
		{
			return Results.Ok(new ConversationMessageDTO
			{
				Id = requestedMessage.Id,
				Sender = requestedMessage.Sender,
				TextContent = requestedMessage.TextContent,
				Timestamp = requestedMessage.Timestamp,
			});
		}
	}

	public async Task<IResult> ProcessUserMessageAsync(int conversationId, PostConversationMessagesDTO payload)
	{
		_logger.LogInformation("Processing incoming user message. ConversationId = {ConvId}, payload = {Payload}", conversationId, payload);
		if (string.IsNullOrWhiteSpace(payload.TextValue))
		{
			return Results.BadRequest(new ErrorDTO { Reason = "The user message does not contain any text." });
		}

		_logger.LogTrace("Searching for the conversation.");
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation == null)
		{
			_logger.LogError("Conversation with ID {ConvId} not found.", conversationId);
			return Results.NotFound(new ErrorDTO { Reason = "Conversation not found." });
		}

		_logger.LogTrace("Adding the user message to the conversation.");
		UserMessage userMessage = await _conversationService.AddNewUserMessageAsync(conversation, payload.TextValue, DateTime.Now);

		// To do: Generate reply here (instead of waiting for a get message call).
		// Todo: Guard this critical section with a semaphore.
		/*_logger.LogTrace("Critical section start: _conversationService.GenerateReplyMessage(userMessage)");
		ReplyMessage? reply = await _conversationService.GenerateReplyMessageAsync(userMessage);
		_logger.LogTrace("Critical section end: _conversationService.GenerateReplyMessage(userMessage)");
		// End of critical section.
		if (reply is null)
		{
			return Results.InternalServerError(new ErrorDTO() { Reason = "An error occured while generating a reply." });
		}
		else
		{
			replyMessage = reply;
		}*/

		return Results.Created(
			$"/conversations/{conversation.Id}/messages/{userMessage.Id}",
			new ConversationMessageDTO
			{
				Id = userMessage.Id,
				Sender = userMessage.Sender,
				TextContent = userMessage.TextContent,
				Timestamp = userMessage.Timestamp,
				ReplyMessageUri = $"/conversations/{conversation.Id}/messages/{userMessage.ReplyMessageId}"
			}
		);
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

		// Make sure the items in the payload are unique
		HashSet<string> uniqueIris = new(payload.ItemIriList);
		_logger.LogTrace("Searching for the selected items.");
		List<DataSpecificationItem> selectedItems = await _database.DataSpecificationItems
					.Where(item => item.DataSpecificationId == conversation.DataSpecification.Id && uniqueIris.Contains(item.Iri))
					.ToListAsync();

		if (selectedItems.Count != uniqueIris.Count)
		{
			List<string> itemsNotFound = uniqueIris.Where(iri => !selectedItems.Any(itemFound => itemFound.Iri == iri)).ToList();
			_logger.LogError("The following item IRIs were not found: {Items}", itemsNotFound);
			return Results.BadRequest(new ErrorDTO() { Reason = "One or more selected items are not present in the data specification." });
		}

		_logger.LogTrace("Generating a suggested message.");
		string? suggestedMessage = await _conversationService.UpdateSelectedItemsAndGenerateSuggestedMessageAsync(conversation, uniqueIris);
		if (string.IsNullOrEmpty(suggestedMessage))
		{
			_logger.LogError("The suggested message is either null or empty.");
			return Results.InternalServerError(new ErrorDTO() { Reason = "There was an error while generating the suggested message." });
		}

		_logger.LogTrace("Returning the suggested message: {SuggestedMessage}", suggestedMessage);
		return Results.Ok(new SuggestedMessageDTO(suggestedMessage));
	}

	public async Task<IResult> DeleteConversationAsync(int conversationId)
	{
		bool result = await _conversationService.DeleteConversationAndAssociatedResourcesAsync(conversationId);
		if (result)
		{
			return Results.Ok();
		}
		else
		{
			return Results.InternalServerError(new ErrorDTO { Reason = "There was an unexpected error while deleting the conversation." });
		}
	}

	private ConversationMessageDTO BuildMessageDTOFromReply(ReplyMessage replyMessage)
	{
		ConversationMessageDTO messageDTO = new();
		messageDTO.Id = replyMessage.Id;
		messageDTO.MappingText = replyMessage.MappingText;

		UserMessage precedingUserMsg = replyMessage.PrecedingUserMessage;
		messageDTO.MappedItems = precedingUserMsg.ItemMappings
			.Select(m => new MappedItemDTO { Iri = m.ItemIri, Label = m.Item.Label, Summary = m.Item.Summary, MappedWords = m.MappedWords })
			.ToList();

		messageDTO.SparqlText = replyMessage.SparqlText;
		messageDTO.SparqlQuery = replyMessage.SparqlQuery;
		messageDTO.SuggestItemsText = replyMessage.SuggestItemsText;

		foreach (DataSpecificationPropertySuggestion suggestion in replyMessage.ItemSuggestions)
		{
			if (suggestion.Item.Type is ItemType.ObjectProperty)
			{
				if (suggestion.RangeItem is null)
				{
					DataSpecificationItem? item = _database.DataSpecificationItems.SingleOrDefault(
						i => i.DataSpecificationId == suggestion.ItemDataSpecificationId && i.Iri == suggestion.RangeItemIri);
					if (item is null)
					{
						// Shouldn't happen but checking just in case.
						throw new Exception("Could not find suggestion range in the database.");
					}
					suggestion.RangeItem = item;
				}
			}
		}

		SuggestionsTransformer transformer = new();
		messageDTO.Suggestions = transformer.TransformSuggestedProperties(replyMessage.ItemSuggestions, replyMessage.Conversation.DataSpecificationSubstructure);
		return messageDTO;
	}
}
