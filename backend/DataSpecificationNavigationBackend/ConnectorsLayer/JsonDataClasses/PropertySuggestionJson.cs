using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record PropertySuggestionJson(
	string Iri,
	string Label,
	ItemType Type,
	string Summary,
	string Reason,
	PropertySuggestionJson.Class DomainClass,
	PropertySuggestionJson.Class RangeClass)
{
	public record Class(
		string Iri,
		string Label,
		string Summary);
}
