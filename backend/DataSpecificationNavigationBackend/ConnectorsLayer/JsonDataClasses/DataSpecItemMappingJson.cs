using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record DataSpecItemMappingJson(
	string Iri,
	string Label,
	ItemType Type,
	string Domain,
	string Range,
	string Summary,
	string MappedWords);
