using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class ConversationService(
	EntityFrameworkPlaceholder entityFrameworkPlaceholder) : IConversationService
{
	private readonly EntityFrameworkPlaceholder _database = entityFrameworkPlaceholder;

	private const string WELCOME_MESSAGE = "Your data specification has been loaded. What would you like to know?";

	public Conversation StartNewConversation(string? conversationTitle, DataSpecification dataSpecification)
	{
		Conversation conversation = new()
		{
			DataSpecification = dataSpecification,
			Title = (conversationTitle ?? "Unnamed conversation"),
		};

		conversation.Messages.Add(new Message()
		{
			Id = 1,
			TextValue = WELCOME_MESSAGE,
			Type = MessageType.WelcomeMessage,
			TimeStamp = DateTime.Now,
		});

		_database.Save(conversation);
		return conversation;
	}

	public IReadOnlyList<Conversation> GetOngoingConversations()
	{
		return _database.GetAllConversations();
	}

	public Conversation? GetConversation(int conversationId)
	{
		return _database.FindConversationById(conversationId);
	}

	public void CreateReplyMessage(Message userMessage)
	{
	}

	public Message AddUserMessage(int conversationId, DateTime timeStamp, string messageText)
	{
		Message userMessage = new()
		{
			Id = 1,
			TextValue = messageText,
			TimeStamp = timeStamp,
			Type = MessageType.UserMessage
		};
		_database.Save(userMessage);
		return userMessage;
	}
}
