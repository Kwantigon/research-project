namespace DataspecNavigationHelper.Model;

public class Message
{
	public int Id; // Should be automatically assigned by the database.

	public MessageType Type;

	public required string TextValue;

	public DateTime TimeStamp;
}

public enum MessageType
{
	UserMessage,
	WelcomeMessage,
	ReplyMessage,
	SuggestedMessage
}
