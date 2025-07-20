using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataspecNavigationBackend.Model;

[PrimaryKey(nameof(Iri), nameof(DataSpecificationId))]
public class DataSpecificationItem
{
	public required string Iri { get; set; }

	public required string Label { get; set; }

	public ItemType Type { get; set; }

	public string? Summary { get; set; }

	public int DataSpecificationId { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
