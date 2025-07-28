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
			DataSpecificationId = dataSpecification.Id,
			DataSpecification = dataSpecification,
			LastUpdated = DateTime.UtcNow,
		};

		Message welcomeMsg = new Message()
		{
			Conversation = conversation,
			ConversationId = conversation.Id,
			TextValue = "Your data specification has been loaded. What would you like to know?",
			Type = MessageType.WelcomeMessage,
			TimeStamp = DateTime.Now
		};
		conversation.Messages.Add(welcomeMsg);

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
				.Include(conversation => conversation.Messages.OrderBy(message => message.TimeStamp))
				.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
		else
		{
			return await _database.Conversations.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
	}

	public async Task<Message> AddNewUserMessage(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage)
	{
		_logger.LogTrace("Creating a new user message object.");
		Message userMessage = new()
		{
			Conversation = conversation,
			ConversationId = conversation.Id,
			TextValue = messageText,
			TimeStamp = timestamp,
			Type = MessageType.UserMessage
		};

		_logger.LogTrace("Creating a new reply message associated with the user message.");
		Message replyMessage = new()
		{
			Type = MessageType.ReplyMessage,
			Conversation = conversation,
			ConversationId = conversation.Id,
			TimeStamp = (DateTime.Now > userMessage.TimeStamp) ? DateTime.Now : DateTime.Now.AddSeconds(1)
		};

		// Calling AddRangeAsync explicitly so that I get generated IDs for the messages.
		_logger.LogTrace("Adding messages to the database.");
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
			List<DataSpecificationItem> mappedItems = await _llmConnector.MapQuestionToItemsAsync(conversation.DataSpecification, userMessage.TextValue);
			_logger.LogDebug("Mapped the question to the following items: {MappedItems}", mappedItems);
			if (mappedItems.Count == 0)
			{
				_logger.LogError("No suitable data specification items found for the question mapping.");
				replyMessage.TextValue = "I could not find anything suitable in the data specification to help with your question.";
				_logger.LogTrace("Not doing any changes to the conversation substructure.");
			}
			else
			{
				_logger.LogTrace("Setting the conversation data spec substructure to the newly mapped items.");
				conversation.DataSpecificationSubstructure = mappedItems;
			}
		}
		else
		{
			_logger.LogTrace("User did not modify the suggested message.");

			if (conversation.UserSelectedItems is null || conversation.UserSelectedItems.Count == 0)
			{
				_logger.LogError("userModifiedSuggestedMessage==false but there are no items for expansion selected by the user in the conversation.");
			}
			else
			{
				_logger.LogTrace("Searching for the items that the user has previously selected.");
				List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecificationId, conversation.UserSelectedItems);
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

	public async Task<Message?> GenerateReplyMessage(Message userMessage)
	{
		_logger.LogTrace("Getting the reply message associated to the user message.");
		Message? replyMessage = userMessage.ReplyMessage;
		if (replyMessage is null)
		{
			_logger.LogError("User message with ID {UserMsgId} does not have an associated reply message.", userMessage.Id);
			return null;
		}

		if (replyMessage.TextValue != string.Empty)
		{
			_logger.LogInformation("The reply message was previously generated already - returning it.");
			return replyMessage;
		}

		// Todo: Generate a Sparql query.
		// Which means I need to implement the Sparql generation.
		_logger.LogTrace("To do: Generating a Sparql query.");
		string sparqlQuery = $"[PLACEHOLDER_SPARQL_QUERY for question \"{userMessage.TextValue}\"]";

		_logger.LogTrace("Getting items related to the question.");
		List<DataSpecificationItem> relatedItems = await _llmConnector.GetRelatedItemsAsync(
			userMessage.Conversation.DataSpecification, userMessage.TextValue, userMessage.Conversation.DataSpecificationSubstructure);
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

			replyMessage.TextValue = $"The data you want can be retrieved using the following sparl query: {sparqlQuery}";
			replyMessage.RelatedItems = relatedItems;
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
			if (conversation.Messages[i].Type is MessageType.UserMessage)
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

		string suggestedMessage = await _llmConnector.GenerateSuggestedMessageAsync(userMessage.TextValue, conversation.DataSpecification, selectedItems, conversation.DataSpecificationSubstructure);
		return suggestedMessage;
	}
}
