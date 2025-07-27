using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.ConnectorsLayer.Abstraction;

public interface ILlmConnector
{
	/// <summary>
	/// Map the concepts in the question in natural language to the items in the data specification.
	/// </summary>
	/// <param name="dataSpecification">The data specification that the question relates to.</param>
	/// <param name="question">The question in natural language.</param>
	/// <returns>Data specification items that correspond to the concepts mentioned in the question.</returns>
	Task<List<DataSpecificationItem>> MapQuestionToItemsAsync(DataSpecification dataSpecification, string question);

	/// <summary>
	/// Given the question and items that have been mapped from the question, get data specification items related to that question.
	/// </summary>
	/// <param name="dataSpecification">The data specification that the question relates to.</param>
	/// <param name="question">The question that was used to get the mapped items.</param>
	/// <param name="mappedItems">Data specification items that the question mapped to.</param>
	/// <returns>Data specification items that somehow relate to the question and can be used to expand the question.</returns>
	Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> mappedItems);

	Task<string> GetItemSummaryAsync(DataSpecificationItem dataSpecificationItem);
}
