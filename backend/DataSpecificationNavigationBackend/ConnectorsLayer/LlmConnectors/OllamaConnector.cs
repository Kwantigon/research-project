﻿using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using OllamaSharp;
using System.Text;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;

public class OllamaConnector(
	ILogger<OllamaConnector> logger)// : ILlmConnector
{
	private readonly ILogger<OllamaConnector> _logger = logger;
	private readonly OllamaPromptConstructor _promptConstructor = new OllamaPromptConstructor();
	private readonly OllamaResponseProcessor _responseProcessor = new OllamaResponseProcessor();

	private OllamaSettings? _ollamaSettings;
	private OllamaApiClient? _ollamaClient;
	private Chat? _chat;

	public Task<string> GenerateItemSummaryAsync(DataSpecificationItem dataSpecificationItem)
	{
		throw new NotImplementedException();
	}

	public Task<string> GenerateSuggestedMessageAsync(string originalQuestion, DataSpecification dataSpecification, List<DataSpecificationItem> selectedItems, List<DataSpecificationItem> currentSubstructure)
	{
		throw new NotImplementedException();
	}

	public Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> mappedItems)
	{
		throw new NotImplementedException();
	}

	public void InitializeModel()
	{
		_ollamaSettings = new OllamaSettings();
		_ollamaClient = new(_ollamaSettings.Uri);
		_ollamaClient.SelectedModel = _ollamaSettings.Model;
		_chat = new(_ollamaClient);
	}

	public async Task<List<DataSpecificationItem>> MapQuestionToItemsAsync(DataSpecification dataSpecification, string question)
	{
		string prompt = _promptConstructor.GetQuestionToItemsMappingPrompt(question);
		string response = await SendPromptAsync(prompt);
		List<DataSpecificationItem> dataSpecificationItems = _responseProcessor.GetDataSpecificationItemsFromResponse(response);
		return dataSpecificationItems;
	}

	private async Task<string> SendPromptAsync(string prompt)
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