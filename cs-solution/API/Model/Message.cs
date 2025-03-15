namespace Backend.Model;

public enum MessageSource
{
	User,
	System
}

public class Message
{
	public uint Id { get; set; }

	public required DateTime TimeStamp { get; set; }

	public required string TextValue { get; set; }
}

public class UserMessage : Message
{
	public SystemMessage? SystemAnswer { get; set; }
}

public class UserPreviewMessage : Message
{

}

public abstract class SystemMessage : Message { }

/// <summary>
/// When the user's query is something completely unrelated to the conversation's data specification,
/// The system will answer with a negative message.
/// </summary>
public class NegativeSystemMessage : SystemMessage { }

public class PositiveSystemMessage : SystemMessage
{
	List<MatchedDataSpecificationProperty> MatchedProperties { get; set; } = [];
}
