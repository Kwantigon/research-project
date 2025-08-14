using DataSpecificationNavigatorBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigatorBackend.Model;
using GenerativeAI;

namespace DataSpecificationNavigatorBackend.ConnectorsLayer.LlmConnectors;

public class GeminiConnector : ILlmConnector
{
	private readonly ILogger<GeminiConnector> _logger;
	private readonly IPromptConstructor _promptConstructor;
	private readonly ILlmResponseProcessor _responseProcessor;
	private const int _retryAttempts = 3; // If the response processor fails to extract the necessary classes, retry the prompt this many times.
	private GenerativeModel? _gemini;

	public GeminiConnector(
		ILogger<GeminiConnector> logger,
		IConfiguration appSettings,
		IPromptConstructor promptConstructor,
		ILlmResponseProcessor responseProcessor)
	{
		_logger = logger;
		_promptConstructor = promptConstructor;
		_responseProcessor = responseProcessor;

		string? apiKeyFile = appSettings["Llm:ApiKeyFile"];
		if (apiKeyFile is null)
		{
			throw new Exception("The key Llm:ApiKeyFile is missing in the config file.");
		}
		string apiKey = File.ReadAllText(apiKeyFile);

		string? model = appSettings["Llm:Model"];
		if (model is null)
		{
			throw new Exception("The key Llm:Model is missing in the config file.");
		}

		GoogleAi googleAi = new(apiKey);
		_gemini = googleAi.CreateGenerativeModel(model);
	}

	public async Task<List<DataSpecificationItemMapping>> MapUserMessageToDataSpecificationAsync(DataSpecification dataSpecification, UserMessage userMessage)
	{
		int attempts = 0;
		List<DataSpecificationItemMapping>? mapped = null;

		_logger.LogTrace("Building a prompt for mapping the question to items.");
		string prompt = _promptConstructor.BuildMapToDataSpecificationPrompt(dataSpecification, userMessage.TextContent);
		//_logger.LogDebug("Map question to items prompt:\n{Prompt}", prompt);

		while (attempts < _retryAttempts && mapped is null)
		{
			_logger.LogTrace("Prompt attempt number {AttemptCount}", attempts + 1);
			string response = await SendPromptAsync(prompt);
			_logger.LogDebug("LLM response: {Response}", response);

			_logger.LogTrace("Extracting the mapped items from the LLM response.");
			mapped = _responseProcessor.ExtractMappedItems(response, userMessage);
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

	public async Task<List<DataSpecificationItemMapping>> MapUserMessageToSubstructureAsync(DataSpecification dataSpecification, DataSpecificationSubstructure substructure, UserMessage userMessage)
	{
		int attempts = 0;
		List<DataSpecificationItemMapping>? mapped = null;

		_logger.LogTrace("Building a prompt for mapping the question to items.");
		string prompt = _promptConstructor.BuildMapToSubstructurePrompt(dataSpecification, userMessage.TextContent, substructure);
		//_logger.LogDebug("Map question to items prompt:\n{Prompt}", prompt);

		while (attempts < _retryAttempts && mapped is null)
		{
			_logger.LogTrace("Prompt attempt number {AttemptCount}", attempts + 1);
			string response = await SendPromptAsync(prompt);
			_logger.LogDebug("LLM response: {Response}", response);

			_logger.LogTrace("Extracting the mapped items from the LLM response.");
			mapped = _responseProcessor.ExtractSubstructureMapping(response, userMessage);
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

	public async Task<List<DataSpecificationPropertySuggestion>> GetSuggestedPropertiesAsync(DataSpecification dataSpecification, DataSpecificationSubstructure substructure, UserMessage userMessage)
	{
		int attempts = 0;
		List<DataSpecificationPropertySuggestion>? suggestedItems = null;

		_logger.LogTrace("Building a prompt for getting the related items.");
		string prompt = _promptConstructor.BuildGetSuggestedItemsPrompt(dataSpecification, userMessage.TextContent, substructure);
		//_logger.LogDebug("Get related items prompt:\n{Prompt}", prompt);

		while (attempts < _retryAttempts && suggestedItems is null)
		{
			_logger.LogTrace("Prompt attempt number {AttemptCount}", attempts + 1);

			_logger.LogTrace("Prompting the LLM.");
			string response = await SendPromptAsync(prompt);

			_logger.LogDebug("LLM response: {Response}", response);

			_logger.LogTrace("Extracting the related items from the LLM response.");
			suggestedItems = _responseProcessor.ExtractSuggestedItems(response, userMessage);
			attempts++;
		}

		if (suggestedItems is null)
		{
			_logger.LogError("The relatedItems list is still null after " + _retryAttempts + " attempts.");
			return [];
		}

		_logger.LogTrace("Returning the related items.");
		return suggestedItems;
	}

	public async Task<string> GenerateSuggestedMessageAsync(DataSpecification dataSpecification, UserMessage userMessage, DataSpecificationSubstructure substructure, List<DataSpecificationItem> selectedItems)
	{
		_logger.LogTrace("Building a prompt for the suggested message.");
		string prompt = _promptConstructor.BuildGenerateSuggestedMessagePrompt(dataSpecification, userMessage.TextContent, substructure, selectedItems);
		//_logger.LogDebug("Generate suggested message:\n{Prompt}", prompt);

		_logger.LogTrace("Prompting the LLM.");
		string response = await SendPromptAsync(prompt);
		_logger.LogDebug("LLM response: {Response}", response);

		_logger.LogTrace("Extracting the suggested message from the LLM response.");
		string? itemSummary = _responseProcessor.ExtractSuggestedMessage(response);
		if (itemSummary is null)
		{
			_logger.LogError("Failed to extract the item summary from the LLM response.");
			return string.Empty;
		}

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

	public async Task<string> GenerateItemSummaryAsync(DataSpecificationItem dataSpecificationItem)
	{
		await Task.CompletedTask;
		return "Item summary generation is deprecated.";
	}
}
