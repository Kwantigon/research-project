using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.ConnectorsLayer.Abstraction;

public interface ILlmConnector
{
	List<DataSpecificationItem> MapQuestionToItems(DataSpecification dataSpecification, string question);
}
