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

		List<ConversationMessageDTO> responseDTO = [];
		foreach (Message msg in conversation.Messages)
		{
			ConversationMessageDTO messageDTO = new()
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

			if (msg is ReplyMessage replyMessage)
			{
				messageDTO.MappingText = replyMessage.MappingText;
				UserMessage? precedingUserMsg = await _conversationService.GetUserMessagePrecedingReplyAsync(replyMessage);
				if (precedingUserMsg is null)
				{
					_logger.LogError("The reply message with ID \"{ReplMsgId}\" does not have a preceding user message.", replyMessage.Id);
					return Results.InternalServerError(new ErrorDTO { Reason = "Found a reply message without the preceding user message." });
				}
				messageDTO.MappedItems = precedingUserMsg.ItemMappingsTable
					.Select(m => new MappedItemDTO { Iri = m.ItemIri, Label = m.Item.Label, Summary = m.Item.Summary, MappedWords = m.MappedWords })
					.ToList();

				messageDTO.SparqlText = replyMessage.SparqlText;
				messageDTO.SparqlQuery = replyMessage.SparqlQuery;
				messageDTO.SuggestItemsText = replyMessage.SuggestItemsText;

				foreach (DataSpecificationItemSuggestion suggestion in replyMessage.ItemSuggestionsTable)
				{
					DataSpecificationItemMapping? mapping = await _database.DataSpecificationItemMappings
					.SingleOrDefaultAsync(m => m.ItemDataSpecificationId == conversation.DataSpecification.Id && m.ItemIri == suggestion.ExpandsItem && m.UserMessageId == precedingUserMsg.Id);
					if (mapping is null)
					{
						_logger.LogError("Could not find the mapping between item \"{ItemIri}\" and the user message \"{UserMsgId}\"", suggestion.ExpandsItem, precedingUserMsg.Id);
					}

					string expandWords = mapping?.MappedWords ?? suggestion.ExpandsItem;

					List<SuggestedItemDTO>? items;
					if (!messageDTO.SuggestedItems.TryGetValue(expandWords, out items))
					{
						items = [];
						messageDTO.SuggestedItems[suggestion.ExpandsItem] = items;
					}
					items.Add(new SuggestedItemDTO()
					{
						Iri = suggestion.ItemIri,
						Label = suggestion.Item.Label,
						Summary = suggestion.Item.Summary,
						Reason = suggestion.ReasonForSuggestion
					});
				}

				responseDTO.Add(messageDTO);
			}
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
			_logger.LogTrace("Searching for the user message preceding this reply message.");
			UserMessage? userMessage = await _conversationService.GetUserMessagePrecedingReplyAsync(replyMessage);
			if (userMessage is null)
			{
				_logger.LogError("The reply message with ID \"{ReplMsgId}\" does not have a preceding user message.", replyMessage.Id);
				return Results.InternalServerError(new ErrorDTO { Reason = "Found a reply message without the preceding user message." });
			}

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

			List<MappedItemDTO> mappedItems = userMessage.ItemMappingsTable
				.Select(mapping => new MappedItemDTO
				{
					Iri = mapping.ItemIri,
					Label = mapping.Item.Label,
					Summary = mapping.Item.Summary,
					MappedWords = mapping.MappedWords
				}).ToList();

			Dictionary<string, List<SuggestedItemDTO>> suggestedItemsPreprocessing = [];
			foreach (var suggestion in replyMessage.ItemSuggestionsTable)
			{
				List<SuggestedItemDTO>? list;
				if (!suggestedItemsPreprocessing.TryGetValue(suggestion.ExpandsItem, out list))
				{
					list = new List<SuggestedItemDTO>();
					suggestedItemsPreprocessing.Add(suggestion.ExpandsItem, list);
				}
				list.Add(new SuggestedItemDTO()
				{
					Iri = suggestion.ItemIri,
					Label = suggestion.Item.Label,
					Summary = suggestion.Item.Summary,
					Reason = suggestion.ReasonForSuggestion
				});
			}

			Dictionary<string, List<SuggestedItemDTO>> suggestedItems = [];
			foreach (var pair in suggestedItemsPreprocessing)
			{
				// pair.Key is the Iri of the item that the suggested items would expand.
				DataSpecificationItemMapping? mapping = await _database.DataSpecificationItemMappings
					.SingleOrDefaultAsync(m => m.ItemDataSpecificationId == conversation.DataSpecification.Id && m.ItemIri == pair.Key && m.UserMessageId == userMessage.Id);
				if (mapping is null)
				{
					_logger.LogError("Could not find the mapping between item \"{ItemIri}\" and the user message \"{UserMsgId}\"", pair.Key, userMessage.Id);
				}

				string expandWords = mapping?.MappedWords ?? pair.Key;
				suggestedItems.Add(expandWords, pair.Value);
			}

			ConversationMessageDTO replyMessageDTO = new()
			{
				Id = replyMessage.Id,
				Timestamp = replyMessage.Timestamp,
				MappingText = replyMessage.MappingText,
				MappedItems = mappedItems,
				SparqlText = replyMessage.SparqlText,
				SuggestItemsText = replyMessage.SuggestItemsText,
				SuggestedItems = suggestedItems
			};
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

		_logger.LogTrace("Searching for the conversation.");
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation == null)
		{
			_logger.LogError("Conversation with ID {ConvId} not found.", conversationId);
			return Results.NotFound(new ErrorDTO { Reason = "Conversation not found." });
		}

		_logger.LogTrace("Adding the user message to the conversation.");
		UserMessage userMessage = await _conversationService.AddNewUserMessageAsync(conversation, payload.TextValue, DateTime.Now, payload.UserModifiedSuggestedMessage);

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

		_logger.LogTrace("Searching for the selected items.");
		List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecification.Id, payload.ItemIriList);
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
		_database.Conversations.Remove(conversation);
		_database.DataSpecifications.Remove(dataSpecification);
		_database.SaveChanges();
		return Results.NoContent();
	}
}
