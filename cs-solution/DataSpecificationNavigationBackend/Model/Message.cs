using System.Text.Json.Serialization;

namespace DataspecNavigationHelper.Model;

public class Message
{
	[JsonPropertyName("id")]
	public int Id { get; set; } // Should be automatically assigned by the database.

	[JsonPropertyName("type")]
	public MessageType Type { get; set; }

	[JsonPropertyName("text")]
	public required string TextValue { get; set; }

	[JsonPropertyName("timestamp")]
	public DateTime TimeStamp { get; set; }

	[JsonPropertyName("relatedItems")]
	public List<DataSpecificationItem>? RelatedItems { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<MessageType>))]
public enum MessageType
{
	UserMessage,
	WelcomeMessage,
	ReplyMessage,
	SuggestedMessage
}
