namespace ConversationsModel;

public class Conversation
{
	private static uint _nextUnusedId = 0;

	public Conversation(DataSpecification dataSpecification)
	{
		Id = _nextUnusedId++;
		DataSpecification = dataSpecification;
		BotMessages = new List<ChatMessage>();
		UserMessages = new List<ChatMessage>();
	}

	public uint Id { get; set; }

	public DataSpecification DataSpecification { get; set; }

	public List<ChatMessage> BotMessages { get; set; }

	public List<ChatMessage> UserMessages { get; set; }
}
