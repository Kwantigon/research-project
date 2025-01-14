using Abstractions;
using Microsoft.SemanticKernel;
using System.Text;

namespace Implementations
{
	public class DummyLlmConnector : ILanguageModelConnector
	{
		public string SendPrompt(string prompt)
		{
			return "I am a dummy connector. Consider your prompt sent.";
		}
	}

	public class OpenAIConnector : ILanguageModelConnector
	{
		public OpenAIConnector()
		{
			IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
			const string CHATGPT_API_KEY = "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";
			const string CHATGPT_MODEL = "gpt-4o-mini";
			kernelBuilder.AddOpenAIChatCompletion(modelId: CHATGPT_MODEL, apiKey: CHATGPT_API_KEY);
			_kernel = kernelBuilder.Build();
		}

		private Kernel _kernel;

		public string SendPrompt(string prompt)
		{
			var promptResponse = _kernel.InvokePromptAsync(prompt).Result;
			return promptResponse.GetValue<string>();
		}
	}

	public class MyCustomLlmConnector : ILanguageModelConnector
	{
		public MyCustomLlmConnector()
		{
			_httpClient = new HttpClient();
		}

		HttpClient _httpClient;

		public string SendPrompt(string prompt)
		{
			var serverResponse = _httpClient.PostAsync("llm.endpoint", new StringContent("{ promptAsJson or some other format }", Encoding.UTF8, "application/json")).Result;
			return serverResponse.Content.ToString();
		}
	}
}
