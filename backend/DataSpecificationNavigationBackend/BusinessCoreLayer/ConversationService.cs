using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

public class ConversationService(
	ILogger<ConversationService> logger,
	AppDbContext appDbContext,
	ILlmConnector llmConnector) : IConversationService
{
	private readonly ILogger<ConversationService> _logger = logger;
	private readonly AppDbContext _database = appDbContext;
	private readonly ILlmConnector _llmConnector = llmConnector;

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

		// ToDo: Add the lazy loading dependency (proxies).
		// Also change the classes in Model to support lazy loading with virtual properties.
		_logger.LogTrace("Mapping the user's question to data specification items.");
		List<DataSpecificationItem> mappedItems = await _llmConnector.MapQuestionToItemsAsync(userMessage.Conversation.DataSpecification, userMessage.TextValue);
		_logger.LogTrace("Mapped the question to {ItemsCount} items.", mappedItems.Count);
		if (mappedItems.Count == 0)
		{
			_logger.LogWarning("No suitable data specification items found for the question mapping.");
			replyMessage.TextValue = "I could not find anything suitable in the data specification to help with your question.";
		}
		else
		{
			// Todo: Update the conversation substructure.
			// Which means I need to add a property that holds the substructure to the conversation.
			_logger.LogTrace("To do: Updating the conversation's data specification substructure.");

			// Todo: Generate a Sparql query.
			// Which means I need to implement the Sparql generation.
			_logger.LogTrace("To do: Generating a Sparql query.");
			string sparqlQuery = $"[PLACEHOLDER_SPARQL_QUERY for question \"{userMessage.TextValue}\"]";

			_logger.LogTrace("Getting items related to the question.");
			List<DataSpecificationItem> relatedItems = await _llmConnector.GetRelatedItemsAsync(
				userMessage.Conversation.DataSpecification, userMessage.TextValue, mappedItems);
			_logger.LogTrace("Found {ItemsCount} related items.", relatedItems.Count);

			replyMessage.TextValue = $"The data you want can be retrieved using the following sparl query: {sparqlQuery}";
			replyMessage.RelatedItems = relatedItems;
		}

		_logger.LogTrace("Saving changes to the database and returning.");
		//await _database.SaveChangesAsync();
		return replyMessage;
	}
}
