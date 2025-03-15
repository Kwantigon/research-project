namespace Backend.DTO;

public class ConversationDTO
{
	/// <summary>
	/// The title of the conversation.
	/// This was either given by the user or it is a default title created by the server.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// The IRI of the conversation on the server.
	/// </summary>
	public required string Location { get; set; }
}
