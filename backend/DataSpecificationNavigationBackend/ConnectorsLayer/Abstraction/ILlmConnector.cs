using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

public interface ILlmConnector
{
	Task<List<DataSpecificationItemMapping>> MapUserMessageToItemsAsync(DataSpecification dataSpecification, UserMessage userMessage);

	Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> currentSubstructure);

	Task<string> GenerateItemSummaryAsync(DataSpecificationItem dataSpecificationItem);

	Task<string> GenerateSuggestedMessageAsync(string originalQuestion, DataSpecification dataSpecification, List<DataSpecificationItem> selectedItems, List<DataSpecificationItem> currentSubstructure);
}
