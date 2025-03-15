using Backend.Abstractions;

namespace Backend.Implementation
{
	public class MockLlmResponseProcessor : ILlmResponseProcessor
	{
		public object ParseMappedDataSpecProperties(string llmResponse)
		{
			throw new NotImplementedException();
		}
	}
}
