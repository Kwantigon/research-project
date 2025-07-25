using System.Text.Json.Serialization;

namespace DataspecNavigationBackend.Model;

public class Message
{
	public Guid Id { get; set; }

	public MessageType Type { get; set; }

	public string TextValue { get; set; } = string.Empty;

	public DateTime TimeStamp { get; set; }

	public virtual List<DataSpecificationItem>? RelatedItems { get; set; }

	public int ConversationId { get; set; }

	public required virtual Conversation Conversation { get; set; }

	public Guid? ReplyMessageId { get; set; }

	public virtual Message? ReplyMessage { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<MessageType>))]
public enum MessageType
{
	UserMessage,
	WelcomeMessage,
	ReplyMessage,
	SuggestedMessage
}
