using Backend.Model;

namespace Backend.Abstractions;

public interface ISparqlTranslationService
{
	string TranslateSubstructure(DataSpecificationSubstructure substructure);
}
