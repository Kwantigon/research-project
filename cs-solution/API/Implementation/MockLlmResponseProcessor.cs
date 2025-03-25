using Backend.Abstractions;

namespace Backend.Implementation
{
	public class MockLlmResponseProcessor : ILlmResponseProcessor
	{
		public object ProcessItemsMappingResponse(string llmResponse)
		{
			throw new NotImplementedException();
		}
	}
}
