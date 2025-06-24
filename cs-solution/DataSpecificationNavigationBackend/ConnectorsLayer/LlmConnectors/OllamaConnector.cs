using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;
using OllamaSharp;
using System.Text;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;

public class OllamaConnector(
	ILogger<OllamaConnector> logger) : ILlmConnector
{
	private readonly ILogger<OllamaConnector> _logger = logger;
	private readonly OllamaPromptConstructor _promptConstructor = new OllamaPromptConstructor();
	private readonly OllamaResponseProcessor _responseProcessor = new OllamaResponseProcessor();

	private OllamaSettings? _ollamaSettings;
	private OllamaApiClient? _ollamaClient;
	private Chat? _chat;

	public void InitializeModel()
	{
		_ollamaSettings = new OllamaSettings();
		_ollamaClient = new(_ollamaSettings.Uri);
		_ollamaClient.SelectedModel = _ollamaSettings.Model;
		_chat = new(_ollamaClient);
	}

	public List<DataSpecificationItem> MapQuestionToItems(string question)
	{
		string prompt = _promptConstructor.GetQuestionToItemsMappingPrompt(question);
		Task<string> task = SendPrompt(prompt);
		task.Wait();
		string response = task.Result;
		List<DataSpecificationItem> dataSpecificationItems = _responseProcessor.GetDataSpecificationItemsFromResponse(response);
		return dataSpecificationItems;
	}

	private async Task<string> SendPrompt(string prompt)
	{
		if (_chat is null)
		{
			throw new Exception("The Ollama chat has not been initialized. Call InitializeModel() first.");
		}
		IAsyncEnumerable<string> responseStream = _chat.SendAsync(prompt);
		StringBuilder responseBuilder = new StringBuilder();

		await foreach(string? token in responseStream)
		{
			responseBuilder.Append(token);
		}
		return responseBuilder.ToString();
	}
}

internal class OllamaPromptConstructor
{
	internal string GetQuestionToItemsMappingPrompt(string question)
	{
		throw new NotImplementedException();
	}
}

internal class OllamaResponseProcessor
{
	internal List<DataSpecificationItem> GetDataSpecificationItemsFromResponse(string response)
	{
		throw new NotImplementedException();
	}
}

internal class OllamaSettings
{
	// ToDo: Should load these things from a config file.

	internal Uri Uri = new("http://localhost:11434");
	internal string Model = "llama3.2:latest";
}