using DataspecNavigationBackend.ConnectorsLayer.Abstraction;
using DataspecNavigationBackend.Model;
using GenerativeAI;
using System.Text;
using System.Text.Json;

namespace DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;

public class GeminiConnector : ILlmConnector
{
	private readonly ILogger<GeminiConnector> _logger;
	private readonly PromptConstructor _promptConstructor;
	private readonly ResponseProcessor _responseProcessor;
	private const int _retryAttempts = 3; // If the response processor fails to extract the necessary classes, retry the prompt this many times.
	private GenerativeModel? _gemini;

	public GeminiConnector(ILogger<GeminiConnector> logger)
	{
		/*_logger = LoggerFactory
											.Create(config => config.SetMinimumLevel(LogLevel.Trace).AddConsole())
											.CreateLogger<GeminiConnector>();*/
		_logger = logger;
		_promptConstructor = new(_logger);
		_responseProcessor = new(_logger);
		string apiKey = File.ReadAllText("C:/Users/nquoc/MatFyz/research-project/repo/research-project/backend/DataSpecificationNavigationBackend/local/gemini_api-key.txt");
		const string model = "models/gemini-2.5-flash";
		GoogleAi googleAi = new(apiKey);
		_gemini = googleAi.CreateGenerativeModel(model);
	}

	public async Task<List<DataSpecificationItem>> MapQuestionToItemsAsync(DataSpecification dataSpecification, string question)
	{
		int attempts = 0;
		List<DataSpecificationItem>? mapped = null;

		_logger.LogTrace("Building a prompt for mapping the question to items.");
		string prompt = _promptConstructor.CreateQuestionToItemsMappingPrompt(question, dataSpecification);
		_logger.LogDebug("Map question to items prompt: \"{Prompt}\"", prompt);

		while (attempts < _retryAttempts && mapped is null)
		{
			_logger.LogTrace("Attempt number {AttemptCount}", attempts + 1);

			_logger.LogTrace("Prompting the LLM.");
			string response = await SendPromptAsync(prompt);
			_logger.LogDebug("LLM response: {Response}", response);

			_logger.LogTrace("Extracting the mapped items from the LLM response.");
			mapped = _responseProcessor.ExtractMappedItems(response, dataSpecification);
			attempts++;
		}

		if (mapped is null)
		{
			_logger.LogError("The data specification items list is still null after " + _retryAttempts + " attempts.");
			throw new Exception("An error occurred while mapping the question to data specification items.");
			// Todo: Gracefully handle the result instead of throwing an exception.
		}

		_logger.LogTrace("Returning the mapped items.");
		return mapped;
	}

	public async Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> mappedItems)
	{
		int attempts = 0;
		List<DataSpecificationItem>? relatedItems = null;

		_logger.LogTrace("Building a prompt for getting the related items.");
		string prompt = _promptConstructor.CreateGetRelatedItemsPrompt(question, dataSpecification, mappedItems);
		_logger.LogDebug("Get related items prompt: \"{Prompt}\"", prompt);

		while (attempts < _retryAttempts && relatedItems is null)
		{
			_logger.LogTrace("Attempt number {AttemptCount}", attempts + 1);

			_logger.LogTrace("Prompting the LLM.");
			string response = await SendPromptAsync(prompt);
			_logger.LogDebug("LLM response: {Response}", response);

			_logger.LogTrace("Extracting the related items from the LLM response.");
			relatedItems = _responseProcessor.ExtractRelatedItems(response, dataSpecification);
			attempts++;
		}

		if (relatedItems is null)
		{
			_logger.LogError("The data relatedItems list is still null after " + _retryAttempts + " attempts.");
			throw new Exception("An error occured while getting the items related to the question.");
			// Todo: Gracefully handle the result instead of throwing an exception.
		}

		_logger.LogTrace("Returning the related items.");
		return relatedItems;
	}

	private async Task<string> SendPromptAsync(string prompt)
	{
		if (_gemini is null)
		{
			throw new Exception("The Gemini model has not been initialized.");
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
			"Iri": "",
			"Type": "",
			"Label": "",
			"Comment": ""
		}}]

		Example output:
		[
			{{
				"Iri": "https://www.example.com/item-one",
				"Type": "Class",
				"Label": "item one",
				"Comment": ""
			}},
			{{
				"Iri": "https://www.example.com/item-two",
				"Type": "ObjectProperty",
				"Label": "item two",
				"Comment": ""
			}},
			{{
				"Iri": "https://www.example.com/item-three",
				"Type": "DatatypeProperty",
				"Label": "item three",
				"Comment": ""
			}}
		]

		Return only the JSON array. Do not surround it with backticks.

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
			"Iri": "",
			"Type": "",
			"Label": "",
			"Comment": ""
		}}]

		Example output:
		[
			{{
				"Iri": "https://www.example.com/item-one",
				"Type": "Class",
				"Label": "item one",
				"Comment": ""
			}},
			{{
				"Iri": "https://www.example.com/item-two",
				"Type": "ObjectProperty",
				"Label": "item two",
				"Comment": ""
			}},
			{{
				"Iri": "https://www.example.com/item-three",
				"Type": "DatatypeProperty",
				"Label": "item three",
				"Comment": ""
			}}
		]

		Return only the JSON array. Do not surround it with backticks.

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

	public List<DataSpecificationItem>? ExtractMappedItems(string llmResponse, DataSpecification dataSpecification)
	{
		return DeserializeDataSpecItems(llmResponse, dataSpecification);
	}

	public List<DataSpecificationItem>? ExtractRelatedItems(string llmResponse, DataSpecification dataSpecification)
	{
		return DeserializeDataSpecItems(llmResponse, dataSpecification);
	}

	private List<DataSpecificationItem>? DeserializeDataSpecItems(string llmResponse, DataSpecification dataSpecification)
	{
		_logger.LogDebug("Extracting items from the following response string:\n{LlmResponse}", llmResponse);
		try
		{
			List<DataSpecificationItem>? items = JsonSerializer.Deserialize<List<DataSpecificationItem>>(llmResponse);
			if (items is null)
			{
				_logger.LogError("Failed to deserialize the LLM response into a list of items.");
			}
			else
			{
				foreach (DataSpecificationItem item in items)
				{
					item.DataSpecificationId = dataSpecification.Id;
					item.DataSpecification = dataSpecification;
				}
			}

			return items;
		}
		catch (JsonException e)
		{
			_logger.LogError("A JsonException occured while extracting the data specification items from the LLM response. {Exception}", e);
			return null;
		}
	}
}
