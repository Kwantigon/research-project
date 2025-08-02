using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

public interface IPromptConstructor
{
	string BuildItemsMappingPrompt(DataSpecification dataSpecification, string userQuestion);

	string BuildGetSuggestedItemsPrompt(DataSpecification dataSpecification, string userQuestion, List<DataSpecificationItem> mappedItems);

	string BuildGenerateSuggestedMessagePrompt(DataSpecification dataSpecification, string userQuestion, List<DataSpecificationItem> mappedItems, List<DataSpecificationItem> selectedItems);
}
