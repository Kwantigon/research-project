using Backend.Model;

namespace Backend.Abstractions.LlmServices;

public interface ILlmResponseProcessor
{
	List<DataSpecificationItem> ProcessItemsMappingResponse(string llmResponse);

	DataSpecificationItemSummary ProcessItemsSummaryResponse(string llmResponse);

	string ProcessQuestionPreviewResponse(string llmResponse);
}
