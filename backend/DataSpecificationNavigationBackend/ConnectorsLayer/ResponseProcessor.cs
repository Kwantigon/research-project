using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.ConnectorsLayer.JsonDataClasses;
using DataSpecificationNavigationBackend.Model;
using System.Text.Json;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class ResponseProcessor(
	ILogger<ResponseProcessor> logger) : ILlmResponseProcessor
{
	private readonly ILogger<ResponseProcessor> _logger = logger;

	public List<DataSpecificationItemMapping>? ExtractMappedItems(string llmResponse, UserMessage userMessage)
	{
		llmResponse = RemoveBackticks(llmResponse.Trim());
		try
		{
			List<ItemMappingJson>? jsonData = JsonSerializer.Deserialize<List<ItemMappingJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemMapping> result = [];
			foreach (ItemMappingJson jsonItem in jsonData)
			{
				DataSpecificationItem dataSpecItem = new()
				{
					Iri = jsonItem.Iri,
					Label = jsonItem.Label,
					Type = jsonItem.Type,
					Summary = jsonItem.Summary,
					DataSpecification = userMessage.Conversation.DataSpecification,
					DataSpecificationId = userMessage.Conversation.DataSpecification.Id
				};

				DataSpecificationItemMapping mapping = new()
				{
					ItemDataSpecificationId = dataSpecItem.DataSpecificationId,
					ItemIri = dataSpecItem.Iri,
					UserMessageId = userMessage.Id,
					Item = dataSpecItem,
					UserMessage = userMessage,
					MappedWords = jsonItem.MappedWords
				};

				result.Add(mapping);
			}
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("An exception occured while deserializing the mapped items from JSON: {Exception}", e);
			return null;
		}
	}

	public List<DataSpecificationItemSuggestion>? ExtractSuggestedItems(string llmResponse, UserMessage userMessage)
	{
		if (userMessage.ReplyMessageId is null || userMessage.ReplyMessage is null)
		{
			// Will need these values later so no point doing anything if they are null.
			_logger.LogError("There is no reply message associated with the user message.");
			return null;
		}

		llmResponse = RemoveBackticks(llmResponse.Trim());
		try
		{
			List<ItemSuggestionJson>? jsonData = JsonSerializer.Deserialize<List<ItemSuggestionJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemSuggestion> result = [];
			foreach (ItemSuggestionJson jsonItem in jsonData)
			{
				DataSpecificationItem dataSpecItem = new()
				{
					Iri = jsonItem.Iri,
					Label = jsonItem.Label,
					Type = jsonItem.Type,
					Summary = jsonItem.Summary,
					DataSpecification = userMessage.Conversation.DataSpecification,
					DataSpecificationId = userMessage.Conversation.DataSpecification.Id
				};

				DataSpecificationItemSuggestion suggestion = new()
				{
					ItemDataSpecificationId = dataSpecItem.DataSpecificationId,
					ItemIri = dataSpecItem.Iri,
					ReplyMessageId = (Guid)userMessage.ReplyMessageId, // Cannot implicitly convert from Guid? to Guid.
					Item = dataSpecItem,
					ReplyMessage = userMessage.ReplyMessage,
					ReasonForSuggestion = jsonItem.Reason,
					ExpandsItem = jsonItem.Expands
				};

				result.Add(suggestion);
			}
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("An exception occured while deserializing the suggested items from JSON: {Exception}", e);
			return null;
		}
	}

	public List<DataSpecificationItemMapping>? ExtractDataSpecSubstructureMapping(string llmResponse, UserMessage userMessage)
	{
		llmResponse = RemoveBackticks(llmResponse.Trim());
		try
		{
			List<ItemMappingForSubstructureJson>? jsonData = JsonSerializer.Deserialize<List<ItemMappingForSubstructureJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemMapping> result = [];
			foreach (ItemMappingForSubstructureJson jsonItem in jsonData)
			{
				DataSpecificationItem? dataSpecItem = userMessage.Conversation.DataSpecificationSubstructure.Find(item => item.Iri == jsonItem.Iri);
				if (dataSpecItem is null)
				{
					_logger.LogError("Item {Iri} is not in the data specification substructure but it was returned as one of the mapped items.", jsonItem.Iri);
					return null;
				}

				DataSpecificationItemMapping mapping = new()
				{
					ItemDataSpecificationId = dataSpecItem.DataSpecificationId,
					ItemIri = dataSpecItem.Iri,
					UserMessageId = userMessage.Id,
					Item = dataSpecItem,
					UserMessage = userMessage,
					MappedWords = jsonItem.MappedWords
				};

				result.Add(mapping);
			}
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("An exception occured while deserializing the mapped items from JSON: {Exception}", e);
			return null;
		}
	}

	private string RemoveBackticks(string llmResponse)
	{
		if (llmResponse.StartsWith("```json"))
		{
			return llmResponse.Substring(7, llmResponse.Length - 10);
		}
		else
		{
			return llmResponse;
		}
	}
}
