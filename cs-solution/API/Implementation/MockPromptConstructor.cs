using Backend.Abstractions;

namespace Backend.Implementation;

public class MockPromptConstructor : IPromptConstructor
{
	public string CreateItemsMappingPrompt(string userQuery)
	{
		throw new NotImplementedException();
	}
}
