﻿using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface ISparqlTranslationService
{
	string TranslateSubstructure(IReadOnlyCollection<DataSpecificationItem> dataSpecificationSubstructure);

	string TranslateSubstructure(DataSpecificationSubstructure substructure);
}
