using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using GenerativeAI;
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
		string prompt = _promptConstructor.CreateMapQuestionToItemsPrompt(question, dataSpecification);
		_logger.LogDebug("Map question to items prompt:\n{Prompt}", prompt);

		while (attempts < _retryAttempts && mapped is null)
		{
			_logger.LogTrace("Prompt attempt number {AttemptCount}", attempts + 1);

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
			return [];
		}

		_logger.LogTrace("Returning the mapped items.");
		return mapped;
	}

	public async Task<List<DataSpecificationItem>> GetRelatedItemsAsync(DataSpecification dataSpecification, string question, List<DataSpecificationItem> currentSubstructure)
	{
		int attempts = 0;
		List<DataSpecificationItem>? relatedItems = null;

		_logger.LogTrace("Building a prompt for getting the related items.");
		string prompt = _promptConstructor.CreateGetRelatedItemsPrompt(question, dataSpecification, currentSubstructure);
		_logger.LogDebug("Get related items prompt:\n{Prompt}", prompt);

		while (attempts < _retryAttempts && relatedItems is null)
		{
			_logger.LogTrace("Prompt attempt number {AttemptCount}", attempts + 1);

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

	public async Task<string> GenerateItemSummaryAsync(DataSpecificationItem dataSpecificationItem)
	{
		_logger.LogTrace("Building a prompt for the item summary.");
		string prompt = _promptConstructor.CreateGenerateItemSummaryPrompt(dataSpecificationItem);
		_logger.LogDebug("Generate item summary prompt:\n{Prompt}", prompt);

		_logger.LogTrace("Prompting the LLM.");
		string response = await SendPromptAsync(prompt);
		_logger.LogDebug("LLM response: {Response}", response);

		_logger.LogTrace("Extracting the item summary from the LLM response.");
		string itemSummary = _responseProcessor.ExtractItemSummary(response);

		_logger.LogTrace("Returning the item summary.");
		return itemSummary;
	}

	public async Task<string> GenerateSuggestedMessageAsync(string originalQuestion, DataSpecification dataSpecification, List<DataSpecificationItem> selectedItems, List<DataSpecificationItem> currentSubstructure)
	{
		_logger.LogTrace("Building a prompt for the suggested message.");
		string prompt = _promptConstructor.CreateGenerateSuggestedMessagePrompt(originalQuestion, dataSpecification, selectedItems, currentSubstructure);
		_logger.LogDebug("Generate suggested message:\n{Prompt}", prompt);

		_logger.LogTrace("Prompting the LLM.");
		string response = await SendPromptAsync(prompt);
		_logger.LogDebug("LLM response: {Response}", response);

		_logger.LogTrace("Extracting the suggested message from the LLM response.");
		string itemSummary = _responseProcessor.ExtractSuggestedMessage(response);

		_logger.LogTrace("Returning the suggested message.");
		return itemSummary;
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
	// {0} = Data specification (OWL file).
	// {1} = User question.
	const string MAP_QUESTION_TO_ITEMS_PROMPT = """
		You are a system that maps natural language questions to items in a structured data specification given as an OWL file.

		### Data specification
		The specification describes entities and their attributes. Here is the OWL file containing the data specification:
		```
		{0}
		```

		### User question
		The question that user asked is:
		"{1}"

		### Task
		Identify which items from the specification are relevant to the question. 
		Return a JSON array corresponding to the following schema:
		[{{
				"Iri": "",
				"Type": "",
				"Label": "",
				"Comment": ""
		}}]

		Example output:
		[
			{{ "Iri": "https://www.example.com/item-one", "Type": "Class", "Label": "item one", "Comment": "" }},
			{{ "Iri": "https://www.example.com/item-two", "Type": "ObjectProperty", "Label": "item two", "Comment": "" }},
			{{ "Iri": "https://www.example.com/item-three", "Type": "DatatypeProperty", "Label": "item three", "Comment": "" }}
		]

		Do not include unrelated items. If the question is not coherent or does not relate to the data specification in any way, return an empty JSON array.
		Return **only JSON** with no extra commentary and do not surround the answer in backticks.
		
		""";

	// {0} = Data specification (OWL file).
	// {1} = User question.
	// {2} = Current data specification substructure in the conversation.
	const string GET_RELATED_ITEMS_PROMPT = """
		You are assisting a user in exploring a data specification.

		### Data specification
		Here is the data specification given as an OWL file.
		```
		{0}
		```

		### User question
		The question that user asked is:
		"{1}"

		### Current context
		The user’s question already involves these items:
		{2}

		### Task
		Suggest other items from the specification that may be relevant to expanding the user’s question.
		Return a JSON array corresponding to the following schema:
		[{{
				"Iri": "",
				"Type": "",
				"Label": "",
				"Comment": ""
		}}]

		Example output:
		[
			{{ "Iri": "https://www.example.com/item-one", "Type": "Class", "Label": "item one", "Comment": "" }},
			{{ "Iri": "https://www.example.com/item-two", "Type": "ObjectProperty", "Label": "item two", "Comment": "" }},
			{{ "Iri": "https://www.example.com/item-three", "Type": "DatatypeProperty", "Label": "item three", "Comment": "" }}
		]

		Do not include unrelated items. If the question is not coherent or does not relate to the data specification in any way, return an empty JSON array.
		Return **only JSON** with no extra commentary and do not surround the answer in backticks.
		
		""";

	// {0} = Data specification (OWL file).
	// {1} = Item iri (from OWL).
	// {2} = Item label (from OWL).
	// {3} = Item type (Class / ObjectProperty / DatatypeProperty).
	const string GENERATE_ITEM_SUMMARY_PROMPT = """
		You are generating a user-friendly summary for a data specification item.

		### Data specification
		Here is the data specification given as an OWL file.
		```
		{0}
		```

		### Item
		Iri: {1}
		Label: {2}
		Type: {3}

		### Task
		Provide a brief summary suitable for non-technical end-users describing the purpose and relevance of this item in the context of the specification.
		Return only the summary and nothing else.
		""";

	// {0} = Original question.
	// {1} = Data specification in OWL.
	// {2} = The current data speification substructure in the conversation.
	// {3} = The items that user selected.

	const string GENERATE_SUGGESTED_MESSAGE_PROMPT = """
		You are assisting an user in refining their data query.

		### Original question
		The user originally asked:
		"{0}"

		### Data specification
		The current data specification is given in the following OWL file
		```
		{1}
		```

		### Current context
		The current query context includes items:
		{2}

		### Newly added items
		The user has now added these items:
		{3}

		### Task
		Generate a new natural language question that integrates the added items 
		smoothly and remains faithful to the user’s intent. Only return the natural language question and nothing else. Do not include any explanations or apologies.
		""";

	private readonly ILogger _logger;

	internal PromptConstructor(ILogger logger)
	{
		_logger = logger;
	}

	public string CreateMapQuestionToItemsPrompt(string question, DataSpecification dataSpecification)
	{
		return string.Format(MAP_QUESTION_TO_ITEMS_PROMPT, dataSpecification.Owl, question);
	}

	public string CreateGetRelatedItemsPrompt(string question, DataSpecification dataSpecification, List<DataSpecificationItem> currentSubstructure)
	{
		return string.Format(GET_RELATED_ITEMS_PROMPT, dataSpecification.Owl, question, string.Join(", ", currentSubstructure.Select(item => item.Iri)));
	}

	public string CreateGenerateItemSummaryPrompt(DataSpecificationItem item)
	{
		return string.Format(GENERATE_ITEM_SUMMARY_PROMPT, item.DataSpecification.Owl, item.Iri, item.Label, item.Type);
	}

	public string CreateGenerateSuggestedMessagePrompt(
		string originalQuestion, DataSpecification dataSpecification,
		List<DataSpecificationItem> selectedItems, List<DataSpecificationItem> currentSubstructure)
	{

		/*StringBuilder substructureSb = new('[');
		string prefix = string.Empty;
		foreach (var item in currentSubstructure)
		{
			substructureSb.Append(prefix);
			substructureSb.Append($"{{ \"iri\": \"{item.Iri}\", \"label\": \"{item.Label}\" }}");
			prefix = ", ";
		}
		substructureSb.Append(']');

		StringBuilder selectedSb = new('[');
		foreach (var item in selectedItems)
		{
			selectedSb.Append(prefix);
			selectedSb.Append($"{{ \"iri\": \"{item.Iri}\", \"label\": \"{item.Label}\" }}");
			prefix = ", ";
		}
		selectedSb.Append(']');
		return string.Format(GENERATE_SUGGESTED_MESSAGE_PROMPT, originalQuestion, dataSpecification.Owl, substructureSb.ToString(), selectedSb.ToString());*/

		string substructureString = string.Join(", ", currentSubstructure.Select(item => item.Iri));
		string selectedString = string.Join(", ", selectedItems.Select(item => item.Iri));
		return string.Format(GENERATE_SUGGESTED_MESSAGE_PROMPT, originalQuestion, dataSpecification.Owl, substructureString, selectedString);
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

	public string ExtractItemSummary(string llmResponse)
	{
		// For now, I will ask the LLM to return the summary directly in the answer.
		// No processing needed to extract the summary.
		// Just return it.
		return llmResponse;
	}

	public string ExtractSuggestedMessage(string llmResponse)
	{
		// For now, I will ask the LLM to return the summary directly in the answer.
		// No processing needed to extract the summary.
		// Just return it.
		return llmResponse;
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
