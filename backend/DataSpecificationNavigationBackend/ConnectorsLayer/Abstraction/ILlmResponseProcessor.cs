using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

/// <summary>
/// Deserializes the LLM response into instances.
/// Stores those instances into the database.
/// </summary>
public interface ILlmResponseProcessor
{
	List<DataSpecificationItemMapping>? ExtractMappedItems(string llmResponse, UserMessage userMessage);

	List<DataSpecificationItemSuggestion>? ExtractSuggestedItems(string llmResponse, UserMessage userMessage);

	string? ExtractSuggestedMessage(string llmResponse) => llmResponse;

	List<DataSpecificationItemMapping>? ExtractSubstructureMapping(string llmResponse, UserMessage userMessage);
}
