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
	public virtual List<ReplyMessage> SuggestedInMessages { get; set; } = [];

	public virtual List<DataSpecificationItemMapping> ItemMappingsTable { get; set; } = [];
	public virtual List<UserMessage> MappedInMessages { get; set; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
