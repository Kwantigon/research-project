using DataspecNavigationBackend.ConnectorsLayer.Abstraction;
using DataspecNavigationBackend.Model;
using GenerativeAI;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;

public class GeminiConnector : ILlmConnector
{
	private readonly ILogger<GeminiConnector> _logger;
	private readonly PromptConstructor _promptConstructor;
	private readonly ResponseProcessor _responseProcessor;
	private const int _retryAttempts = 3; // If the response processor fails to extract the necessary classes, retry the prompt this many times.
	private string? _apiKey;
	private string? _model;
	private GenerativeModel? _gemini;

	public GeminiConnector()
	{
		_logger = LoggerFactory
											.Create(config => config.SetMinimumLevel(LogLevel.Trace).AddConsole())
											.CreateLogger<GeminiConnector>();
		_promptConstructor = new(_logger);
		_responseProcessor = new(_logger);
		_apiKey = File.ReadAllText("C:/Users/nquoc/MatFyz/research-project/repo/research-project/cs-solution/DataSpecificationNavigationBackend/local/gemini_api-key.txt");
		_model = "models/gemini-2.5-flash";
		GoogleAi googleAi = new(_apiKey);
		_gemini = googleAi.CreateGenerativeModel(_model);
	}

	public async Task<List<DataSpecificationItem>> MapQuestionToItemsAsync(DataSpecification dataSpecification, string question)
	{
		/*int attempts = 0;
		List<DataSpecificationItem>? items = null;

		while (attempts < _retryAttempts && items is null)
		{
			string prompt = _promptConstructor.CreateQuestionToItemsMappingPrompt(question, dataSpecification);
			string response = await SendPromptAsync(prompt);
			items = _responseProcessor.ExtractDataSpecificationItems(response, dataSpecification);
			attempts++;
		}
		
		if (items is null)
		{
			_logger.LogError("The data specification items list is still null after " + _retryAttempts + " attempts.");
			throw new Exception("An error occurred while mapping the question to data specification items.");
		}
		return items;*/

		// Mock
		await Task.CompletedTask;
		return [
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item one",
				Summary = "AAAAAaaaaa",
				Type = ItemType.Class
			},
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item two",
				Summary = "bbbbb",
				Type = ItemType.DatatypeProperty
			},
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item three",
				Summary = "CCccCC",
				Type = ItemType.ObjectProperty
			}
		];
	}

	public async Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> mappedItems)
	{
		/*int attempts = 0;
		List<DataSpecificationItem>? relatedItems = null;

		while (attempts < _retryAttempts && relatedItems is null)
		{
			string prompt = _promptConstructor.CreateGetRelatedItemsPrompt(question, dataSpecification, mappedItems);
			string response = await SendPromptAsync(prompt);
			relatedItems = _responseProcessor.ExtractRelatedItems(response, dataSpecification);
			attempts++;
		}

		if (relatedItems is null)
		{
			_logger.LogError("The data relatedItems list is still null after " + _retryAttempts + " attempts.");
			throw new Exception("An error occured while getting the items related to the question.");
		}
		return relatedItems;*/

		// Mock
		await Task.CompletedTask;
		return [
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item one",
				Summary = "AAAAAaaaaa",
				Type = ItemType.Class
			},
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item two",
				Summary = "bbbbb",
				Type = ItemType.DatatypeProperty
			},
			new DataSpecificationItem()
			{
				DataSpecification = dataSpecification,
				DataSpecificationId = dataSpecification.Id,
				Iri = $"mock-item.{Guid.NewGuid()}",
				Label = "Mock item three",
				Summary = "CCccCC",
				Type = ItemType.ObjectProperty
			}
		];
	}

	private async Task<string> SendPromptAsync(string prompt)
	{
		if (_gemini is null)
		{
			throw new Exception("The Gemini model has not been initialized. Call the Initialize() method first.");
		}
		var response = await _gemini.GenerateContentAsync(prompt);
		return response.Text;
	}
}

internal class PromptConstructor
{
	const string MAP_QUESTION_TO_ITEMS_PROMPT = """
		Given an OWL file describing a data specification and a question in natural language, map the question to relevant OWL entities and return them in a JSON according to the following schema:
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

	const string GET_RELATED_ITEMS_PROMPT = """
		Given an OWL file describing a data specification, question in natural language and the OWL entities mapped from the question to the data specification, give me 5 entities from the data specification that are not the mapped entities and are relevant to the original question. Return them in a JSON according to the following schema:
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

		The mapped entities are
		```
		{1}
		```

		The question is
		```
		{2}
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

	public string CreateGetRelatedItemsPrompt(string question, DataSpecification dataSpecification, List<DataSpecificationItem> relatedItems)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string prefix = string.Empty;
		foreach (DataSpecificationItem item in relatedItems)
		{
			stringBuilder.Append(prefix);
			prefix = ", ";
			stringBuilder.Append(item.Iri);
		}
		return string.Format(GET_RELATED_ITEMS_PROMPT, dataSpecification.Owl, stringBuilder.ToString(), question);
	}
}

internal class ResponseProcessor
{
	private readonly ILogger _logger;

	internal ResponseProcessor(ILogger logger)
	{
		_logger = logger;
	}

	public List<DataSpecificationItem>? ExtractDataSpecificationItems(string llmResponse, DataSpecification dataSpecification)
	{
		_logger.LogDebug("Extracting items from the following response string:\n{LlmResponse}", llmResponse);
		try
		{
			List<DataSpecificationItem>? mappedItems = JsonSerializer.Deserialize<List<DataSpecificationItem>>(llmResponse);
			if (mappedItems != null)
			{
				foreach (DataSpecificationItem item in mappedItems)
				{
					item.DataSpecificationId = dataSpecification.Id;
					item.DataSpecification = dataSpecification;
				}
			}
			return mappedItems;
		}
		catch (JsonException e)
		{
			_logger.LogError("A JsonException occured while extracting the data specification items from the LLM's response. {Exception}", e);
			return null;
		}
	}

	public List<DataSpecificationItem>? ExtractRelatedItems(string llmResponse, DataSpecification dataSpecification)
	{
		// Mock items.
		throw new NotImplementedException();
	}
}
