using Backend.Model;

namespace Backend.Abstractions;

public interface ILlmResponseProcessor
{
	List<DataSpecificationItem> ProcessItemsMappingResponse(string llmResponse);

	DataSpecificationItemSummary ProcessItemsSummaryResponse(string llmResponse);
}
