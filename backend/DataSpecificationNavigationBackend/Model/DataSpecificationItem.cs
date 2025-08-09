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

	public int DataSpecificationId { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }

	public virtual List<DataSpecificationItemSuggestion> ItemSuggestionsTable { get; set; } = [];

	public virtual List<DataSpecificationItemMapping> ItemMappingsTable { get; set; } = [];

	/// <summary>
	/// IRI of the DataSpecificationItem that is the domain for this item.<br/>
	/// (only if this item is of type ObjectProperty or DatatypeProperty)
	/// </summary>
	public string? Domain { get; set; }

	/// <summary>
	/// Type of the item. It can be one of:<br/>
	/// - IRI of a DataSpecificationItem (in case of an ObjectProperty).<br/>
	/// - The string "rdfs:Literal" or something similar (in case of a DatatypeProperty).
	/// </summary>
	public string? Range { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class,
	ObjectProperty, // Relation between classes: class -> class.
	DatatypeProperty // Relation between a class and a literal: class -> literal.
}
