using Backend.DTO;

namespace Backend.Model;

public class Message
{
	public uint Id { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }
}

public class UserMessage : Message
{
	public SystemAnswer? SystemAnswer { get; set; }
}

public class UserPreviewMessage : Message
{
}

public class PlainTextSystemMessage : Message { }

public abstract class SystemAnswer : Message { }

/// <summary>
/// When the user's query is something completely unrelated to the conversation's data specification,
/// The system will answer with a negative message.
/// </summary>
public class NegativeSystemAnswer : SystemAnswer { }

public class PositiveSystemAnswer : SystemAnswer
{
	List<MatchedDataSpecificationItem> MatchedItems { get; set; } = [];
}
