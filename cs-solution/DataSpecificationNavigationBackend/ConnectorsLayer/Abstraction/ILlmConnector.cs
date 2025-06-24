using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.ConnectorsLayer.Abstraction;

public interface ILlmConnector
{
	List<DataSpecificationItem> MapQuestionToItems(DataSpecification dataSpecification, string question);
}
