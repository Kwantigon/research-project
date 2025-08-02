using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

public class ConversationService(
	ILogger<ConversationService> logger,
	AppDbContext appDbContext,
	ILlmConnector llmConnector,
	IDataSpecificationService dataSpecificationService) : IConversationService
{
	private readonly ILogger<ConversationService> _logger = logger;
	private readonly AppDbContext _database = appDbContext;
	private readonly ILlmConnector _llmConnector = llmConnector;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;

	public async Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification)
	{
		Conversation conversation = new()
		{
			Title = conversationTitle,
			DataSpecification = dataSpecification
		};

		Message welcomeMsg = new Message()
		{
			Sender = Message.Source.System,
			TextContent = "Your data specification has been loaded. What would you like to know?",
			Timestamp = DateTime.Now,
			Conversation = conversation
		};
		conversation.Messages.Add(welcomeMsg);
		conversation.LastUpdated = welcomeMsg.Timestamp;

		await _database.Messages.AddAsync(welcomeMsg);
		await _database.Conversations.AddAsync(conversation);
		await _database.SaveChangesAsync();
		_logger.LogDebug("New conversation created and stored successfully.");
		return conversation;
	}

	public async Task<IReadOnlyList<Conversation>> GetAllConversationsAsync()
	{
		_logger.LogDebug("Getting all conversations from the database.");
		return await _database.Conversations
			.Include(c => c.DataSpecification) // Eager load the data specifications because I know they will be needed.
			.ToListAsync();
	}

	public async Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false)
	{
		_logger.LogDebug($"Getting conversation with ID={conversationId} from the database.");
		if (includeMessages)
		{
			return await _database.Conversations
				.Include(conversation => conversation.Messages.OrderBy(message => message.Timestamp))
				.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
		else
		{
			return await _database.Conversations.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
	}

	public async Task<UserMessage> AddNewUserMessageAsync(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage)
	{
		_logger.LogTrace("Creating a new user message object.");
		UserMessage userMessage = new()
		{
			Sender = Message.Source.User,
			TextContent = messageText,
			Timestamp = timestamp,
			Conversation = conversation
		};

		_logger.LogTrace("Creating a new reply message associated with the user message.");
		ReplyMessage replyMessage = new()
		{
			Sender = Message.Source.System,
			Timestamp = (DateTime.Now > userMessage.Timestamp) ? DateTime.Now : userMessage.Timestamp.AddSeconds(1),
			Conversation = conversation
		};

		// Calling AddRangeAsync explicitly so that I get generated IDs for the messages.
		_logger.LogTrace("Adding messages to the database to retrieve their IDs.");
		await _database.Messages.AddRangeAsync([userMessage, replyMessage]);
		conversation.Messages.Add(userMessage);
		conversation.Messages.Add(replyMessage);

		_logger.LogTrace("Setting the reply message and its ID to the user message.");
		userMessage.ReplyMessageId = replyMessage.Id;
		userMessage.ReplyMessage = replyMessage;

		if (userModifiedSuggestedMessage)
		{
			_logger.LogTrace("User has modified the suggested message (or this is the first user message in the conversation).");

			_logger.LogTrace("Mapping the question to data specification items.");
			List<DataSpecificationItemMapping> mappings = await _llmConnector.MapQuestionToItemsAsync(conversation.DataSpecification, userMessage.TextContent);
			_logger.LogDebug("Mapped the question to the following items: {MappedItems}", mappings);
			if (mappings.Count == 0)
			{
				_logger.LogError("No suitable data specification items found for the question mapping.");
				replyMessage.TextContent = "I could not find anything suitable in the data specification to help with your question.";
				_logger.LogTrace("Not doing any changes to the conversation substructure.");
			}
			else // mappings.Count > 0
			{
				_logger.LogTrace("Setting the conversation data spec substructure and mappings in data spec item and message.");
				foreach (DataSpecificationItemMapping mapping in mappings)
				{
					// I'm assuming all fields of the mapping are already set.
					conversation.DataSpecificationSubstructure.Add(mapping.Item);

					mapping.Item.MappedInMessages.Add(mapping.UserMessage);
					mapping.Item.ItemMappingsTable.Add(mapping);

					//mapping.UserMessage.MappedItems.Add(mapping.Item);
					mapping.UserMessage.ItemMappingsTable.Add(mapping);
				}
			}
		}
		else // userModifiedSuggestedMessage is false
		{
			_logger.LogTrace("User did not modify the suggested message.");

			if (conversation.UserSelectedItems is null || conversation.UserSelectedItems.Count == 0)
			{
				_logger.LogError("userModifiedSuggestedMessage==false but there are no items for expansion selected by the user in the conversation.");
			}
			else
			{
				_logger.LogTrace("Searching for the items that the user has previously selected.");
				List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecification.Id, conversation.UserSelectedItems);
				_logger.LogDebug("conversation.UserSelectedItems.Count = {SelectedCount}, selectedItems.Count = {SelectedFound}", conversation.UserSelectedItems.Count, selectedItems.Count);

				_logger.LogTrace("Filtering the selected items - keeping only those that are not already in the conversation data spec substructure.");
				List<DataSpecificationItem>  itemsNotInConversation = selectedItems.Where(selected => !conversation.DataSpecificationSubstructure.Any(i => i.Iri == selected.Iri)).ToList();

				_logger.LogTrace("Adding the selected items to the conversation.");
				conversation.DataSpecificationSubstructure.AddRange(itemsNotInConversation);
			}
		}

		_logger.LogTrace("Saving changes to the database and returning.");
		await _database.SaveChangesAsync();
		return userMessage;
	}

	public async Task<ReplyMessage?> GenerateReplyMessageAsync(UserMessage userMessage)
	{
		_logger.LogTrace("Getting the reply message associated to the user message.");
		ReplyMessage? replyMessage = userMessage.ReplyMessage;
		if (replyMessage is null)
		{
			_logger.LogError("User message with text \"{UserMsgText}\" does not have an associated reply message.", userMessage.TextContent);
			return null;
		}

		if (replyMessage.IsGenerated)
		{
			_logger.LogInformation("The reply message was previously generated already - returning it.");
			return replyMessage;
		}

		// Item mappings answer.
		/*List<DataSpecificationItemMapping> mappings = await _database.DataSpecificationItemMappings
																																.Where(mapping => mapping.UserMessageId == userMessage.Id)
																																.ToListAsync();*/
		if (userMessage.ItemMappingsTable.Count > 0)
		{
			replyMessage.MappingText = "I have identified the following items from your data specification which play a role in your question:";
		}

		// Todo: Generate a Sparql query.
		// Which means I need to implement the Sparql generation.
		_logger.LogTrace("To do: Generating a Sparql query.");
		string sparqlQuery = $"[PLACEHOLDER_SPARQL_QUERY for question \"{userMessage.TextContent}\"]";

		_logger.LogTrace("Getting items related to the question.");
		List<DataSpecificationItem> relatedItems = await _llmConnector.GetRelatedItemsAsync(
			userMessage.Conversation.DataSpecification, userMessage.TextContent, userMessage.Conversation.DataSpecificationSubstructure);
		_logger.LogTrace("Found {ItemsCount} related items.", relatedItems.Count);

		// Todo: Change this quick and dirty solution.
		// Check if any of the related items is already in the database.
		// If yes, take that item from the database instead.
		// This is to prevent _database.SaveChangesAsync from throwing an exception that some item IRIs are already in the database.
		for (int i = 0; i < relatedItems.Count; i++)
		{
			{
				DataSpecificationItem item = relatedItems[i];
				DataSpecificationItem? fromDb = await _database.DataSpecificationItems.SingleOrDefaultAsync(
					i => i.DataSpecificationId == item.DataSpecificationId && i.Iri == item.Iri);

				if (fromDb != null)
				{
					relatedItems[i] = fromDb;
				}
			}

			replyMessage.TextContent = $"The data you want can be retrieved using the following sparl query: {sparqlQuery}";
		}

		_logger.LogTrace("Saving changes to the database and returning.");
		await _database.SaveChangesAsync();
		return replyMessage;
	}

	public async Task<string?> GenerateSuggestedMessageAsync(Conversation conversation, List<DataSpecificationItem> selectedItems)
	{
		// Todo: Add log trace.
		Message? userMessage = null;
		for (int i = conversation.Messages.Count - 1; i >= 0; i--)
		{
			if (conversation.Messages[i] is UserMessage)
			{
				userMessage = conversation.Messages[i];
				break;
			}
		}
		if (userMessage is null)
		{
			_logger.LogError("The conversation does not contain any user messages.");
			return null;
		}

		//string suggestedMessage = await _llmConnector.GenerateSuggestedMessageAsync(userMessage.TextContent, conversation.DataSpecification, selectedItems, conversation.DataSpecificationSubstructure);
		string suggestedMessage = "LLM call is commented out.";
		return suggestedMessage;
	}

	public async Task<UserMessage?> GetUserMessagePrecedingReplyAsync(ReplyMessage replyMessage)
	{
		return await (_database.UserMessages.SingleOrDefaultAsync(msg => msg.ReplyMessageId == replyMessage.Id));
	}
}
