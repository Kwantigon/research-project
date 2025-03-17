namespace Backend.DTO;

public enum MessageType
{
	UserMessage,
	SystemMessagePlainText,
	NegativeSystemAnswer,
	PositiveSystemAnswer
}

public class MessageDetailedDTO
{
	public required MessageType Type { get; set; }

	public uint Id { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }

	public SystemAnswerDTO? SystemAnswer { get; set; }
}

public class SystemAnswerDTO
{
	public required MessageType AnswerType { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }

	public List<MatchedDataSpecificationItem>? MatchedDataSpecificationItems { get; set; }
}

public class MatchedDataSpecificationItem
{
	/// <summary>
	/// The unique ID under which the item is saved.
	/// I don't know if it'll be a string or something else.
	/// </summary>
	public required string ItemId { get; set; }

	/// <summary>
	/// The index where the item starts in the user's query original query.
	/// </summary>
	public int StartingIndex { get; set; }

	/// <summary>
	/// The length of the item in the user's original query.
	/// </summary>
	public int Length { get; set; }
}
