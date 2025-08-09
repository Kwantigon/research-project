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
			List<DataSpecItemMappingJson>? jsonData = JsonSerializer.Deserialize<List<DataSpecItemMappingJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemMapping> result = [];
			foreach (DataSpecItemMappingJson jsonItem in jsonData)
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
					MappedWords = jsonItem.MappedWords,
					IsSelectTarget = jsonItem.IsSelectTarget
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
			List<PropertySuggestionJson>? jsonData = JsonSerializer.Deserialize<List<PropertySuggestionJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemSuggestion> result = [];
			foreach (PropertySuggestionJson jsonItem in jsonData)
			{
				if (jsonItem.Type is ItemType.Class)
				{
					_logger.LogError("LLM response contains a suggested item of type Class: {0}.", jsonItem);
					continue;
				}

				DataSpecificationItem suggestedProperty = new()
				{
					Iri = jsonItem.Iri,
					Label = jsonItem.Label,
					Type = jsonItem.Type,
					Summary = jsonItem.Summary,
					DataSpecification = userMessage.Conversation.DataSpecification,
					DataSpecificationId = userMessage.Conversation.DataSpecification.Id,
					DomainItemIri = jsonItem.DomainClass.Iri,
					RangeItemIri = jsonItem.RangeClass.Iri
				};
				DataSpecificationItem domainItem = new()
				{
					Iri = jsonItem.DomainClass.Iri,
					Label = jsonItem.DomainClass.Label,
					Type = ItemType.Class,
					Summary = jsonItem.DomainClass.Summary,
					DataSpecification = userMessage.Conversation.DataSpecification,
					DataSpecificationId = userMessage.Conversation.DataSpecification.Id
				};

				DataSpecificationItem? rangeItem = null;
				if (jsonItem.Type is ItemType.ObjectProperty)
				{
					rangeItem = new()
					{
						Iri = jsonItem.RangeClass.Iri,
						Label = jsonItem.RangeClass.Label,
						Type = ItemType.Class,
						Summary = jsonItem.RangeClass.Summary,
						DataSpecification = userMessage.Conversation.DataSpecification,
						DataSpecificationId = userMessage.Conversation.DataSpecification.Id
					};
				}

				DataSpecificationItemSuggestion suggestion = new()
				{
					ItemDataSpecificationId = suggestedProperty.DataSpecificationId,
					ItemIri = suggestedProperty.Iri,
					ReplyMessageId = (Guid)userMessage.ReplyMessageId, // Explicit conversion because the compiler says "cannot implicitly convert from Guid? to Guid".
					Item = suggestedProperty,
					ReplyMessage = userMessage.ReplyMessage,
					ReasonForSuggestion = jsonItem.Reason,
					DomainItemIri = jsonItem.DomainClass.Iri,
					DomainItem = domainItem,
					RangeItemIri = jsonItem.RangeClass.Iri,
					RangeItem = rangeItem
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

	public List<DataSpecificationItemMapping>? ExtractSubstructureMapping(string llmResponse, UserMessage userMessage)
	{
		llmResponse = RemoveBackticks(llmResponse.Trim());
		try
		{
			List<SubstructureItemMappingJson>? jsonData = JsonSerializer.Deserialize<List<SubstructureItemMappingJson>>(llmResponse);
			if (jsonData is null)
			{
				_logger.LogError("The result of the JSON deserialization is null.");
				return null;
			}

			List<DataSpecificationItemMapping> result = [];
			List<object> substructureItems = new();
			foreach (var classItem in userMessage.Conversation.DataSpecificationSubstructure.ClassItems)
			{
				substructureItems.Add(new DataSpecificationSubstructure.ClassItem()
				{
					Iri = classItem.Iri,
					Label = classItem.Label,
					IsSelectTarget = classItem.IsSelectTarget,
					DatatypeProperties = null!,
					ObjectProperties = null!
				});
				substructureItems.AddRange(classItem.ObjectProperties);
				substructureItems.AddRange(classItem.DatatypeProperties);
			}
			foreach (SubstructureItemMappingJson jsonItem in jsonData)
			{
				DataSpecificationItem placeholderItem = new()
				{
					Iri = jsonItem.Iri,
					Label = "Placeholder item from substructure mapping",
					DataSpecification = null!
				};


				DataSpecificationItemMapping mapping = new()
				{
					ItemDataSpecificationId = userMessage.Conversation.DataSpecification.Id,
					ItemIri = jsonItem.Iri,
					UserMessageId = userMessage.Id,
					Item = placeholderItem,
					// This placeholderItem reference will be replaced in conversation service where the mapping references are updated.
					// The assumption is that when the mapping to substructure methods are called, the conversation substructure already contains that item.
					// So this reference will be replaced later by a reference to the actual item in the database.
					UserMessage = userMessage,
					MappedWords = jsonItem.MappedWords,
					IsSelectTarget = jsonItem.IsSelectTarget
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
