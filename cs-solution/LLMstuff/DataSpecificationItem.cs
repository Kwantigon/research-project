using System.Text.Json.Serialization;

namespace LLMstuff;

public class DataSpecificationItem
{
	[JsonPropertyName("iri")]
	public required string Iri { get; init; }

	[JsonPropertyName("label")]
	public required string Label { get; init; }

	[JsonPropertyName("type")]
	public required ItemType Type { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
