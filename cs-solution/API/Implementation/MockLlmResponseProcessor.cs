using BackendApi.Abstractions;

namespace BackendApi.Implementation
{
	public class MockLlmResponseProcessor : ILlmResponseProcessor
	{
		static private MockLlmResponseProcessor? _instance = null;

		public static MockLlmResponseProcessor GetInstance()
		{
			if (_instance == null)
				return new MockLlmResponseProcessor();
			else return _instance;
		}

		private MockLlmResponseProcessor() { }

		public object ParseMappedDataSpecProperties(string llmResponse)
		{
			Console.WriteLine("MockLlmResponseProcessor::ParseMappedDataSpecProperties() invoked.");
			return string.Empty;
		}
	}
}
