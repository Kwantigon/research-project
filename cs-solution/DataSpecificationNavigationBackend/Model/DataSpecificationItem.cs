using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataspecNavigationHelper.Model;

public class DataSpecificationItem
{
	[JsonConstructor]
	public DataSpecificationItem(string iri, string label, ItemType type, string? summary = null)
	{
		Iri = iri;
		Label = label;
		Type = type;
		Summary = summary;
	}

	[Key]
	[JsonPropertyName("iri")]
	public string Iri { get; set; }

	[JsonPropertyName("label")]
	public string Label { get; set; }

	[JsonPropertyName("type")]
	public ItemType Type { get; set; }

	public string? Summary { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
