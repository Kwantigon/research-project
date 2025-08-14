namespace DataSpecificationNavigatorBackend.Model;

public class Conversation
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public DateTime LastUpdated { get; set; }

	public virtual required DataSpecification DataSpecification { get; set; }

	// Messages are ordered by Timestamp, so the most recent message is last.
	private List<Message> _messages = [];
	public virtual List<Message> Messages
	{
		get => _messages.ToList(); // Return a copy to avoid external modification.
		set => _messages = value
			.OrderBy(m => m.Timestamp)
			.ToList();
	}

	// DataSpecificationSubstructure

	public List<string> UserSelectedItems { get; set; } = []; // IRIs of items.

	public string? SuggestedMessage { get; set; }

	public void AddMessage(Message message)
	{
		ArgumentNullException.ThrowIfNull(message);

		message.Conversation = this; // Ensure the message knows its conversation. But this is not strictly necessary.
		if (_messages.Count > 0)
		{
			Message lastMessage = _messages.Last();
			if (lastMessage.Timestamp > message.Timestamp)
			{
				throw new ArgumentException("Message timestamp must be greater than the last message's timestamp.");
			}
		}
		_messages.Add(message);
		LastUpdated = message.Timestamp;
	}
}
