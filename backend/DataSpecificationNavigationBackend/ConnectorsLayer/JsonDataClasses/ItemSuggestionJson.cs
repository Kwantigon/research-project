using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record ItemSuggestionJson(
	string Iri,
	string Label,
	ItemType Type,
	string Comment,
	string Summary,
	string Reason,
	string Expands);
