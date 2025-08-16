using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DataSpecificationNavigatorBackend.Model;

[PrimaryKey(nameof(DataSpecificationId), nameof(Iri))]
public abstract class DataSpecificationItem
{
	public required string Iri { get; set; }

	public required string Label { get; set; }

	public virtual ItemType Type { get; }

	public string? Summary { get; set; }

	public required int DataSpecificationId { get; set; } // For Entity Framework configuration.

	public virtual required DataSpecification DataSpecification { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class,
	ObjectProperty,
	DatatypeProperty
}

public class ClassItem : DataSpecificationItem
{
	public override ItemType Type { get => ItemType.Class; }
}

public class PropertyItem : DataSpecificationItem
{
	public required string DomainIri { get; set; } // For Entity Framework configuration.

	public virtual required ClassItem Domain { get; set; }
}

public class ObjectPropertyItem : PropertyItem
{
	public override ItemType Type { get => ItemType.ObjectProperty; }

	public required string RangeIri { get; set; } // For Entity Framework configuration.

	public virtual required ClassItem Range { get; set; }
}

public class DatatypePropertyItem : PropertyItem
{
	public override ItemType Type { get => ItemType.DatatypeProperty; }

	public required string RangeDatatypeIri { get; set; }
}
