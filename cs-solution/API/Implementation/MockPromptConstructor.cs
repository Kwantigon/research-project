using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class MockPromptConstructor : IPromptConstructor
{
	public string CreateItemsMappingPrompt(string userQuery)
	{
		return "Mock items mapping prompt.";
	}

	public string CreateItemSummaryPrompt(DataSpecificationItem dataSpecificationItem)
	{
		return "Mock item summary prompt.";
	}

	public string CreateQuestionPreviewPrompt(DataSpecificationSubstructure substructure)
	{
		return "Mock question preview prompt.";
	}
}
