using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(Iri), nameof(DataSpecificationId))]
public class DataSpecificationItem
{
	public string Iri { get; set; }

	public string Label { get; set; }

	public ItemType Type { get; set; }

	public string? Summary { get; set; }

	public int DataSpecificationId { get; set; }

	public virtual DataSpecification DataSpecification { get; set; }

	public virtual List<DataSpecificationPropertySuggestion> ItemSuggestionsTable { get; set; } = [];

	public virtual List<DataSpecificationItemMapping> ItemMappingsTable { get; set; } = [];

	public string? DomainItemIri { get; set; }

	public virtual DataSpecificationItem DomainItem { get; set; }

	public string? RangeItemIri { get; set; }

	public virtual DataSpecificationItem? RangeItem { get; set; } // For now, If I need range, I will always need to look it up from the database.
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class,
	ObjectProperty, // Relation between classes: class -> class.
	DatatypeProperty // Relation between a class and a literal: class -> literal.
}
