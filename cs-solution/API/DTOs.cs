namespace DTO;

public record ChatMessage(
	string MessageFrom,
	string MessageText
);

public class UserQuery
{
	public string? Query { get; set; }

	public override string ToString()
	{
		return Query == null ? "null" : Query;
	}
}

public class DataSpecificationUrl
{
	public string? Url { get; set; }

	public override string ToString()
	{
		return Url == null ? "null" : Url;
	}
}

public class ExpandQueryDTO
{
	public ExpandQueryDTO(IReadOnlyList<string> properties, string query)
	{
		this.Properties = properties;
		this.QueryToExpand = query;
	}

	public IReadOnlyList<string> Properties { get; set; } = [];

	public string QueryToExpand { get; set; }
}
