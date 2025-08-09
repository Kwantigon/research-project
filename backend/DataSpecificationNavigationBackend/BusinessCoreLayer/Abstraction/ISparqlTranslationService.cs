using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface ISparqlTranslationService
{
	string TranslateSubstructure(DataSpecificationSubstructure substructure);
}
