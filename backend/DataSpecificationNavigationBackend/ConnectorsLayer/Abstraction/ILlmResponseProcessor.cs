using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

/// <summary>
/// Deserializes the LLM response into instances.
/// Stores those instances into the database.
/// </summary>
public interface ILlmResponseProcessor
{
	List<DataSpecificationItemMapping>? ExtractMappedItems(string llmResponse, Conversation conversation);
}
