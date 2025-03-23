using System.Text.Json.Serialization;

namespace Backend.DTO;

public class MessageBasicDTO
{
	public required MessageSource Source { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string Text { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<MessageSource>))]
public enum MessageSource
{
	User,
	System
}
