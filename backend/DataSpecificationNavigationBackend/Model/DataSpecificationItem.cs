using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(Iri), nameof(DataSpecificationId))]
public class DataSpecificationItem
{
	public required string Iri { get; set; }

	public required string Label { get; set; }

	public ItemType Type { get; set; }

	public string? Summary { get; set; }

	public string? Comment { get; set; }

	public int DataSpecificationId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public virtual DataSpecification DataSpecification { get; set; }
	// Not using the "required" keyword for this property.
	// It causes trouble when deserializing from the LLM response to an item.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
