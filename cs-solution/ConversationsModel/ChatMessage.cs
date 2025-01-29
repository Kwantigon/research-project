namespace ConversationsModel;

public class ChatMessage
{
	public uint Id { get; set; }

	public DateTime TimeStamp { get; set; }

	public string Message { get; set; } = string.Empty;
}

public class UserMessage : ChatMessage
{
}
