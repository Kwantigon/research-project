using Backend.Model;

namespace Backend.Abstractions;

public interface IPromptConstructor
{
	/// <summary>
	/// Construct a prompt to map the concepts from the user's query
	/// to the relevant properties in the data specification.
	/// </summary>
	/// <param name="userQuery">The user's query in natural language</param>
	/// <returns>A prompt ready to be sent to the LLM.</returns>
	string CreateItemsMappingPrompt(string userQuery);

	string CreateItemSummaryPrompt(DataSpecificationItem dataSpecificationItem);
}
