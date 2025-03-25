using System.Text.Json.Serialization;

namespace Backend.DTO;

[JsonConverter(typeof(JsonStringEnumConverter<MessageType>))]
public enum MessageType
{
	UserMessage,
	SystemMessagePlainText,
	NegativeSystemAnswer,
	PositiveSystemAnswer
}

public class MessageDetailedDTO
{
	public MessageType Type { get; set; }

	public required uint Id { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? SystemAnswerIri { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public List<DataSpecificationItemDTO>? MatchedDataSpecificationItems { get; set; }
}

