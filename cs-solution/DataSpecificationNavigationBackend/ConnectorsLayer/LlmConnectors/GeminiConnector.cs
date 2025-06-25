using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;
using GenerativeAI;
using GenerativeAI.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;

public class GeminiConnector(
	ILogger<GeminiConnector> logger) : ILlmConnector
{
	private readonly ILogger<GeminiConnector> _logger = logger;
	private readonly PromptConstructor _promptConstructor = new(logger);
	private readonly ResponseProcessor _responseProcessor = new(logger);
	private const int _retryAttempts = 3; // If the response processor fails to extract the necessary classes, retry the prompt this many times.
	private string? _apiKey;
	private string? _model;
	private GenerativeModel? _gemini;

	[MemberNotNull(nameof(_gemini))]
	public void Initialize()
	{
		LoadLlmSettings();
		GoogleAi googleAi = new(_apiKey);
		_gemini = googleAi.CreateGenerativeModel(_model);
	}

	public List<DataSpecificationItem> MapQuestionToItems(DataSpecification dataSpecification, string question)
	{
		int attempts = 0;
		List<DataSpecificationItem>? items = null;

		while (attempts < _retryAttempts && items is null)
		{
			string prompt = _promptConstructor.CreateQuestionToItemsMappingPrompt(question, dataSpecification);
			string response = SendPrompt(prompt);
			items = _responseProcessor.ExtractDataSpecificationItems(response);
			attempts++;
		}
		
		if (items is null)
		{
			_logger.LogError("The data specification items list is still null after " + _retryAttempts + " attempts.");
			throw new Exception("There was an error that prevented mapping data specification items to the question.");
		}
		return items;
	}

	private string SendPrompt(string prompt)
	{
		if (_gemini is null)
		{
			throw new Exception("The Gemini model has not been initialized. Call the Initialize() method first.");
		}
		Task<GenerateContentResponse> task = _gemini.GenerateContentAsync(prompt);
		task.Wait();
		return task.Result.Text;
	}

	[MemberNotNull(nameof(_apiKey), nameof(_model))]
	private void LoadLlmSettings()
	{
		_apiKey = "";
		_model = "models/gemini-2.5-flash";
	}
}

internal class PromptConstructor
{
	const string MAP_QUESTION_TO_ITEMS_PROMPT = """
		Given an OWL file describing a data specification and a question in natural language, map the question to relevant OWL entities and return then in a JSON according to the following schema:
		[{{
			"iri": "",
			"type": "",
			"label": "",
			"comment": ""
		}}]

		Example output:
		[
			{{
				"iri": "https://www.example.com/item-one",
				"type": "Class",
				"label": "item one",
				"comment": ""
			}},
			{{
				"iri": "https://www.example.com/item-two",
				"type": "ObjectProperty",
				"label": "item two",
				"comment": ""
			}},
			{{
				"iri": "https://www.example.com/item-three",
				"type": "DatatypeProperty",
				"label": "item three",
				"comment": ""
			}}
		]

		The OWL file is
		```
		{0}
		```

		The question is
		```
		{1}
		```
		""";

	private readonly ILogger _logger;

	internal PromptConstructor(ILogger logger)
	{
		_logger = logger;
	}

	public string CreateQuestionToItemsMappingPrompt(string question, DataSpecification dataSpecification)
	{
		return string.Format(MAP_QUESTION_TO_ITEMS_PROMPT, dataSpecification.Owl, question);
	}
}

internal class ResponseProcessor
{
	private readonly ILogger _logger;

	internal ResponseProcessor(ILogger logger)
	{
		_logger = logger;
	}

	public List<DataSpecificationItem>? ExtractDataSpecificationItems(string llmResponse)
	{
		try
		{
			return JsonSerializer.Deserialize<List<DataSpecificationItem>>(llmResponse);
		}
		catch (JsonException e)
		{
			_logger.LogError("A JsonException occured while extracting the data specification items from the LLM's response: {}", e);
			return null;
		}
	}
}
