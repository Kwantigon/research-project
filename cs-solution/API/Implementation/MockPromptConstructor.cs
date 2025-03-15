using Backend.Abstractions;

namespace Backend.Implementation;

public class MockPromptConstructor : IPromptConstructor
{
	public string ConstructQueryToDataSpecPropertiesPrompt(string userQuery)
	{
		throw new NotImplementedException();
	}
}
