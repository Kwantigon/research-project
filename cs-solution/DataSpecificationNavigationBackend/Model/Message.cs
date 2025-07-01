using System.Text.Json.Serialization;

namespace DataspecNavigationHelper.Model;

public class Message
{
	[JsonPropertyName("id")]
	public int Id { get; init; } // Should be automatically assigned by the database.

	[JsonPropertyName("type")]
	public MessageType Type { get; init; }

	[JsonPropertyName("textValue")]
	public required string TextValue { get; init; }

	[JsonPropertyName("timestamp")]
	public DateTime TimeStamp { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter<MessageType>))]
public enum MessageType
{
	UserMessage,
	WelcomeMessage,
	ReplyMessage,
	SuggestedMessage
}
