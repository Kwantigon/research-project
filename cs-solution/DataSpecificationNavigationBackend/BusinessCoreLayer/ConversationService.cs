using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.BusinessCoreLayer.Abstraction;
using DataspecNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataspecNavigationBackend.BusinessCoreLayer;

public class ConversationService(
	ILogger<ConversationService> logger,
	AppDbContext appDbContext) : IConversationService
{
	private readonly ILogger<ConversationService> _logger = logger;
	private readonly AppDbContext _database = appDbContext;

	private const string WELCOME_MESSAGE = "Your data specification has been loaded. What would you like to know?";

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
			TextValue = WELCOME_MESSAGE,
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
		return await _database.Conversations.Include(c => c.DataSpecification).ToListAsync();
	}

	public async Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false)
	{
		_logger.LogDebug($"Getting conversation with ID={conversationId} from the database.");
		if (includeMessages)
		{
			return await _database.Conversations
				.Include(conversation => conversation.Messages)
				.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		} else
		{
			return await _database.Conversations.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
	}

	public async Task<Message> AddNewUserMessage(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage)
	{
		Message message = new()
		{
			Conversation = conversation,
			ConversationId = conversation.Id,
			TextValue = messageText,
			TimeStamp = timestamp,
			Type = MessageType.UserMessage
		};

		Message replyMessage = new()
		{
			Conversation = conversation,
			ConversationId = conversation.Id,
			Type = MessageType.ReplyMessage
		};

		await _database.Messages.AddRangeAsync([message, replyMessage]);
		conversation.Messages.Add(message);
		conversation.Messages.Add(replyMessage);
		return message;

		// Todo: Possibly start doing stuff for the reply message here.
		// Or wait until a GET request for the reply message is received.
	}
}
