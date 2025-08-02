using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record ItemMappingJsonClass(
	string? Iri,
	string? Label,
	ItemType Type,
	string Comment,
	string Summary,
	string MappedWords);
