using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class MockLlmConnector : ILlmConnector
{
	public string SendPromptAndReceiveResponse(string prompt)
	{
		return "Mock LLM response.";
	}
}
