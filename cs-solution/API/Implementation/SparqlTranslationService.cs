using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class SparqlTranslationService : ISparqlTranslationService
{
	public string TranslateSubstructure(DataSpecificationSubstructure substructure)
	{
		return "Mock SPARQL query";
	}
}
