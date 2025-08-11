using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;

public record DataSpecItemMappingJson(
	string Iri,
	string Summary,
	string MappedWords,
	bool IsSelectTarget = false);
