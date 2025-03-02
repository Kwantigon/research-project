namespace BackendApi.Model;

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
	public SystemMessage? ResponseFromSystem { get; set; }
}

public class SystemMessage : Message
{
	public string? SparqlQuery { get; set; }

	public IList<string> HighlightedWords { get; set; } = new List<string>();
}
