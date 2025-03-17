namespace Backend.Model;

public class Conversation
{
	private static uint _nextUnusedConversationId = 0;

	private uint _nextUnusedMessageId = 0;

	private enum ConversationState
	{
		NotInitialized,
		AwaitingFirstUserMessage,
		AwaitingFollowUpUserMessage
	}

	private ConversationState _state = ConversationState.NotInitialized;

	public Conversation(DataSpecification dataSpecification, string? title)
	{
		Id = _nextUnusedConversationId++;
		Title = title != null ? title : $"Untitled conversation #{Id}";
		DataSpecification = dataSpecification;
		Messages = new List<Message>();
	}

	public uint Id { get; }

	public string Title { get; set; }

	public DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; }

	public DataSpecificationSubstructure? DataSpecificationSubstructure { get; set; }

	public UserPreviewMessage? NextQuestionPreview { get; set; }

	public UserMessage AddUserMessage(string text, DateTime timestamp)
	{
		if (_state is not ConversationState.AwaitingFirstUserMessage)
		{
			throw new InvalidOperationException("The conversation is not accepting user messages at this time.");
		}

		var msg = new UserMessage()
		{
			Id = _nextUnusedMessageId++,
			TimeStamp = timestamp,
			TextValue = text
		};
		Messages.Add(msg);

		_state = ConversationState.AwaitingFollowUpUserMessage;
		return msg;
	}

	/// <summary>
	/// Initialize the conversation by adding the very first system message,
	/// which asks the user, what they would like to be answered.
	/// </summary>
	public void InitializeConversation()
	{
		Messages.Add(new PlainTextSystemMessage()
		{
			Id = _nextUnusedMessageId++,
			TimeStamp = DateTime.Now,
			TextValue = "Your data specification has been loaded.\nWhat would you like to know?"
		});
		_state = ConversationState.AwaitingFirstUserMessage;
	}
}

