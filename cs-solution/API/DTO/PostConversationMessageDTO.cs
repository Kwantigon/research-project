namespace Backend.DTO;

public class PostConversationMessageDTO
{
	public int Source { get; set; }

	public required DateTime TimeStamp { get; set; }

	public string? TextValue { get; set; }
}
