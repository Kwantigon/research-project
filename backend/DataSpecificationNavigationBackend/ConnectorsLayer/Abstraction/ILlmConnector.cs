using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

public interface ILlmConnector
{
	Task<List<DataSpecificationItemMapping>> MapUserMessageToItemsAsync(DataSpecification dataSpecification, UserMessage userMessage);

	Task<List<DataSpecificationItemSuggestion>> GetSuggestedItemsAsync(DataSpecification dataSpecification, UserMessage userMessage, List<DataSpecificationItem> currentSubstructure);

	Task<string> GenerateItemSummaryAsync(DataSpecificationItem dataSpecificationItem);

	Task<string> GenerateSuggestedMessageAsync(DataSpecification dataSpecification, UserMessage userMessage, List<DataSpecificationItem> currentSubstructure, List<DataSpecificationItem> selectedItems);

	Task<List<DataSpecificationItemMapping>> MapUserMessageToConversationDataSpecSubstructureAsync(UserMessage userMessage);
}
