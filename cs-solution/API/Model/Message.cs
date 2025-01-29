namespace BackendApi.Model;

public class Message
{
	// Messages might also have to contain ID.
	// For now they don't. I'll see later.

	public required MessageSource Source { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }
}

public enum MessageSource
{
	ChatBot,
	User
}
