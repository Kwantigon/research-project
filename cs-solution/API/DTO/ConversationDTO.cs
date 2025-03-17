using Backend.Model;

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

	/// <summary>
	/// The IRI of the data specification used for this conversation.
	/// </summary>
	public required string DataSpecificationIri { get; set; }

	/// <summary>
	/// The number of messages in the conversation.
	/// </summary>
	public int MessageCount { get; set; }

	public static implicit operator ConversationDTO(Conversation conversation)
	{
		return new ConversationDTO()
		{
			Title = conversation.Title,
			Location = $"/conversations/{conversation.Id}",
			DataSpecificationIri = $"/data-specifications/{conversation.DataSpecification.Id}",
			MessageCount = conversation.Messages.Count
		};
	}
}
