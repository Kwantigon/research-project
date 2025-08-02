using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record ItemMappingJson(
	string Iri,
	string Label,
	ItemType Type,
	string Comment,
	string Summary,
	string MappedWords);
