namespace BackendApi.DTO;

public class PostConversationMessageDTO
{
	/// <summary>
	/// Either user's message or chatbot's message.
	/// 0 = user's message, 1 = chatbot's message.
	/// </summary>
	public int Source { get; set; }

	public required DateTime TimeStamp { get; set; }

	public string? TextValue { get; set; }
}
