using BackendApi.Model;

namespace BackendApi.Abstractions;

public interface ILlmResponseProcessor
{
	SparqlResponse ProcessSparqlTranslationQuery(string sparqlTranslationResponse);

	PropertySummary ProcessPropertySummaryResponse(string propertySummaryResponse);
}
