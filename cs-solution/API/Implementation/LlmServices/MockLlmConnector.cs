using Backend.Abstractions.LlmServices;
using Backend.Model;

namespace Backend.Implementation.LlmServices;

public class MockLlmConnector : ILlmConnector
{
	public string SendPromptAndReceiveResponse(string prompt)
	{
		return "Mock LLM response.";
	}
}
