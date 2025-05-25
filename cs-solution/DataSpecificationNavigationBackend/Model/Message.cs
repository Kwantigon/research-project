namespace DataspecNavigationHelper.Model;

public record Message(MessageType Type, int Id, string TextValue, DateTime TimeStamp);

public enum MessageType
{
	UserMessage,
	WelcomeMessage,
	ReplyMessage,
	SuggestedMessage
}
