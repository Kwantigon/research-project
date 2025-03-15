namespace Backend.DTO;

public class PostConversationsRequestDTO
{
	/// <summary>
	/// The data specification that will be used for the conversation.
	/// </summary>
	public required string DataSpecificationUri { get; set; }

	/// <summary>
	/// The title to store the conversation under.
	/// </summary>
	public string? ConversationTitle { get; set; }
}
