namespace Backend.Model;

public class Conversation
{
	private static uint _nextUnusedConversationId = 0;

	public Conversation(DataSpecification dataSpecification, string? title)
	{
		Id = _nextUnusedConversationId++;
		Title = title != null ? title : $"Untitled conversation #{Id}";
		DataSpecification = dataSpecification;
		Messages = new List<Message>();
	}

	public uint NextUnusedMessageId { get; set; } = 0;

	public uint Id { get; }

	public string Title { get; set; }

	public DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; }

	public DataSpecificationSubstructure? DataSpecificationSubstructure { get; set; }

	public UserPreviewMessage? NextQuestionPreview { get; set; }

	public DataSpecificationSubstructure? NextQuestionSubstructurePreview { get; set; }

	public ConversationState State { get; set; } = ConversationState.NotInitialized;

	/// <summary>
	/// Initialize the conversation by adding the very first system message,
	/// which asks the user, what they would like to be answered.
	/// </summary>
	public void InitializeConversation()
	{
		Messages.Add(new PlainTextSystemMessage
		{
			Id = NextUnusedMessageId++,
			TimeStamp = DateTime.Now,
			TextValue = "Your data specification has been loaded.\nWhat would you like to know?"
		});
		State = ConversationState.AwaitingFirstUserMessage;
	}
}

public enum ConversationState
{
	NotInitialized,
	AwaitingFirstUserMessage,
	AwaitingFollowUpUserMessage
}
