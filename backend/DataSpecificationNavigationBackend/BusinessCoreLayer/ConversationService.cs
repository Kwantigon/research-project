using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

public class ConversationService(
	ILogger<ConversationService> logger,
	AppDbContext appDbContext,
	ILlmConnector llmConnector,
	IDataSpecificationService dataSpecificationService,
	ISparqlTranslationService sparqlTranslationService) : IConversationService
{
	#region Private fields
	private readonly ILogger<ConversationService> _logger = logger;
	private readonly AppDbContext _database = appDbContext;
	private readonly ILlmConnector _llmConnector = llmConnector;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	private readonly ISparqlTranslationService _sparqlTranslationService = sparqlTranslationService;
	#endregion Private fields

	public async Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification)
	{
		Conversation conversation = new()
		{
			Title = conversationTitle,
			DataSpecification = dataSpecification
		};

		Message welcomeMsg = new Message()
		{
			Sender = Message.Source.System,
			TextContent = "Your data specification has been loaded. What would you like to know?",
			Timestamp = DateTime.Now,
			Conversation = conversation
		};
		conversation.Messages.Add(welcomeMsg);
		conversation.LastUpdated = welcomeMsg.Timestamp;

		await _database.Messages.AddAsync(welcomeMsg);
		await _database.Conversations.AddAsync(conversation);
		await _database.SaveChangesAsync();
		_logger.LogDebug("New conversation created and stored successfully.");
		return conversation;
	}

	public async Task<IReadOnlyList<Conversation>> GetAllConversationsAsync()
	{
		_logger.LogDebug("Getting all conversations from the database.");
		return await _database.Conversations
			.Include(c => c.DataSpecification) // Eager load the data specifications because I know they will be needed.
			.ToListAsync();
	}

	public async Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false)
	{
		_logger.LogDebug($"Getting conversation with ID={conversationId} from the database.");
		if (includeMessages)
		{
			return await _database.Conversations
				.Include(conversation => conversation.Messages.OrderBy(message => message.Timestamp))
				.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
		else
		{
			return await _database.Conversations.SingleOrDefaultAsync(conv => conv.Id == conversationId);
		}
	}

	public async Task<UserMessage> AddNewUserMessageAsync(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage)
	{
		_logger.LogTrace("Creating a new user message object.");
		UserMessage userMessage = new()
		{
			Sender = Message.Source.User,
			TextContent = messageText,
			Timestamp = timestamp,
			Conversation = conversation
		};

		_logger.LogTrace("Creating a new reply message associated with the user message.");
		ReplyMessage replyMessage = new()
		{
			Sender = Message.Source.System,
			Timestamp = (DateTime.Now > userMessage.Timestamp) ? DateTime.Now : userMessage.Timestamp.AddSeconds(1),
			Conversation = conversation,
			PrecedingUserMessage = userMessage
		};

		// Calling AddAsync explicitly so that I get generated IDs for the messages.
		_logger.LogTrace("Adding messages to the database to retrieve their IDs.");
		await _database.UserMessages.AddAsync(userMessage);
		await _database.ReplyMessages.AddAsync(replyMessage);
		conversation.Messages.Add(userMessage);
		conversation.Messages.Add(replyMessage);

		_logger.LogTrace("Setting the reply message and its ID to the user message.");
		userMessage.ReplyMessageId = replyMessage.Id;
		userMessage.ReplyMessage = replyMessage;

		if (string.IsNullOrEmpty(conversation.SuggestedMessage) || userMessage.TextContent.ToLower() != conversation.SuggestedMessage.ToLower())
		{
			_logger.LogTrace("User has modified the suggested message (or this is the first user message in the conversation).");

			_logger.LogTrace("Mapping the question to data specification items.");
			List<DataSpecificationItemMapping> mappings = await MapToDataSpecificationAsync(conversation.DataSpecification, userMessage);
			conversation.DataSpecificationSubstructure = new();
			if (mappings.Count == 0)
			{
				_logger.LogError("No suitable data specification items found for the question mapping.");
			}
			else // mappings.Count > 0
			{
				_logger.LogTrace("Adding mapped items to the conversation substructure.");
				AddMappedItemsToSubstructure(conversation.DataSpecificationSubstructure, mappings);
			}
		}
		else // User sent the suggested message as is, without any modifications.
		{
			_logger.LogTrace("User did not modify the suggested message.");
			if (conversation.UserSelectedItems.Count == 0)
			{
				_logger.LogError("User sent the suggested message but there are no items for expansion selected by the user in the conversation.");
				// I don't know yet what to do in this case.
				// Do nothing for now.
			}
			else
			{
				_logger.LogTrace("Searching for the items that the user has previously selected.");
				List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecification.Id, conversation.UserSelectedItems);

				//_logger.LogTrace("Filtering the selected items - keeping only those that are not already in the conversation data spec substructure.");
				//List<DataSpecificationItem> itemsNotInConversation = selectedItems.Where(selected => !conversation.DataSpecificationSubstructure.Any(i => i.Iri == selected.Iri)).ToList();

				_logger.LogTrace("Adding the selected items to the conversation.");
				AddSelectedItemsToSubstructure(conversation.DataSpecificationSubstructure, selectedItems);

				// Do the mapping for items.
				//List<DataSpecificationItemMapping> mappings = await _llmConnector.MapUserMessageToConversationDataSpecSubstructureAsync(userMessage);
				List<DataSpecificationItemMapping> mappings = await MapToSubstructureAsync(conversation.DataSpecification, conversation.DataSpecificationSubstructure, userMessage);

				// To do: Stejne dostanu mappings kdyz volam MapToSubstructureAsync.
				// Mohl bych udelat mapping a pak volat stejnou metodu jako predtim, tedy AddMappedItemsToSubstructure
				// Jen se mi nelibi, ze nazev neni tolik vypovidajici.
				// Tady je jasne videt, ze nejdriv pridam do substructure a potom jen ziskam mapping pro slova.
				// Funguje to, takze to zatim necham. Az budu delat code polish, tak se k tomu to vratim.
				// To, ze map to substructure vraci ItemMapping se i hodi na to, abych si tam ulozil, jestli novy class item je select target nebo ne.
			}
		}

		conversation.UserSelectedItems?.Clear();
		conversation.SuggestedMessage = null;
		_logger.LogTrace("Saving changes to the database and returning.");
		await _database.SaveChangesAsync();
		return userMessage;
	}

	public async Task<ReplyMessage?> GenerateReplyMessageAsync(UserMessage userMessage)
	{
		_logger.LogTrace("Getting the reply message associated to the user message.");
		ReplyMessage? replyMessage = userMessage.ReplyMessage;
		if (replyMessage is null)
		{
			_logger.LogError("User message with text \"{UserMsgText}\" does not have an associated reply message.", userMessage.TextContent);
			return null;
		}

		if (replyMessage.IsGenerated)
		{
			_logger.LogInformation("The reply message was previously generated already - returning it.");
			return replyMessage;
		}

		if (userMessage.ItemMappings.Count > 0)
		{
			replyMessage.MappingText = "I have identified the following items from your data specification which play a role in your question.";
			replyMessage.SparqlText = "I have formulated a Sparql query for your question:";
			// I assume that userMessage is the most recent user message in the conversation.
			_logger.LogTrace("Generating a Sparql query.");
			replyMessage.SparqlQuery = _sparqlTranslationService.TranslateSubstructure(userMessage.Conversation.DataSpecificationSubstructure);

			_logger.LogTrace("Getting item suggestions.");
			List<DataSpecificationItemSuggestion> suggestions = await GetSuggestionsAsync(
				userMessage.Conversation.DataSpecification, userMessage.Conversation.DataSpecificationSubstructure, userMessage);
			_logger.LogTrace("The LLM suggested {ItemsCount} items.", suggestions.Count);

			if (suggestions.Count == 0)
			{
				replyMessage.SuggestItemsText = "Unfortunately, I did not manage to find any suitable items to suggest to you to further expand your question.";
			}
			else
			{
				replyMessage.SuggestItemsText = "I found some items which could expand your question.";
			}
			replyMessage.IsGenerated = true;
		}
		else
		{
			replyMessage.TextContent = "Sorry, I did not manage to find anything from the data specification that matches your question.";
			replyMessage.IsGenerated = true;
		}
		_logger.LogTrace("Saving changes to the database and returning.");
		await _database.SaveChangesAsync();
		return replyMessage;
	}

	public async Task<string?> UpdateSelectedItemsAndGenerateSuggestedMessageAsync(Conversation conversation, List<DataSpecificationItem> selectedItems)
	{
		_logger.LogTrace("Searching for the most recent user message.");
		UserMessage? userMessage = null;
		for (int i = conversation.Messages.Count - 1; i >= 0; i--)
		{
			if (conversation.Messages[i] is UserMessage)
			{
				userMessage = (UserMessage?)conversation.Messages[i];
				break;
			}
		}
		if (userMessage is null)
		{
			_logger.LogError("The conversation does not contain any user messages.");
			conversation.UserSelectedItems?.Clear();
			return null;
		}

		conversation.UserSelectedItems.Clear();
		foreach (DataSpecificationItem selected in selectedItems)
		{
			switch (selected.Type)
			{
				case ItemType.Class:
					if (!conversation.DataSpecificationSubstructure.ClassItems.Any(c => c.Iri == selected.Iri))
					{
						conversation.UserSelectedItems.Add(selected.Iri);
					}
					break;
				case ItemType.ObjectProperty:
				case ItemType.DatatypeProperty:
					if (selected.DomainItemIri is null || selected.RangeItemIri is null)
					{
						_logger.LogError("Selected item's domain: {Domain}, range: {Range}", selected.DomainItemIri, selected.RangeItemIri);
						throw new Exception("Selected item's domain or range is null");
					}

					conversation.UserSelectedItems.Add(selected.Iri);
					if (!conversation.DataSpecificationSubstructure.ClassItems.Any(c => c.Iri == selected.DomainItemIri))
					{
						conversation.UserSelectedItems.Add(selected.DomainItemIri);
					}
					if (!conversation.DataSpecificationSubstructure.ClassItems.Any(c => c.Iri == selected.RangeItemIri))
					{
						conversation.UserSelectedItems.Add(selected.RangeItemIri);
					}
					break;
			}
		}

		string suggestedMessage = await _llmConnector.GenerateSuggestedMessageAsync(conversation.DataSpecification, userMessage, conversation.DataSpecificationSubstructure, selectedItems);
		conversation.SuggestedMessage = suggestedMessage;

		await _database.SaveChangesAsync();
		return suggestedMessage;
	}

	public async Task<bool> DeleteConversationAndAssociatedResourcesAsync(int conversationId)
	{
		Conversation? conversation = await _database.Conversations.SingleOrDefaultAsync(c => c.Id == conversationId);
		if (conversation is null)
		{
			_logger.LogError("Conversation with ID {Id} not found. But it's OK because there is nothing to delete.", conversationId);
			return true;
		}

		_database.Messages.RemoveRange(conversation.Messages);
		_database.DataSpecifications.Remove(conversation.DataSpecification);
		_database.Conversations.Remove(conversation);
		try
		{
			await _database.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError("An exception occured while saving to the database: {Ex}", e);
			return false;
		}
		return true;
	}

	#region Private methods

	private void StoreMappingsAndUpdateItsReferences(List<DataSpecificationItemMapping> mappings)
	{
		if (mappings.Count == 0)
			return;

		foreach (DataSpecificationItemMapping mapping in mappings)
		{
			// Check the database and conversation substructure for the item.
			DataSpecificationItem? item = _database.DataSpecificationItems
				.SingleOrDefault(item => item.DataSpecificationId == mapping.ItemDataSpecificationId && item.Iri == mapping.ItemIri);
			if (item is not null)
			{
				// Change the reference to the actual item from the database.
				// So that there is no duplicate item conflict when I save later.
				mapping.Item = item;
			}

			mapping.Item.ItemMappingsTable.Add(mapping);
			mapping.UserMessage.ItemMappings.Add(mapping);
		}
	}

	private async Task<List<DataSpecificationItemMapping>> MapToDataSpecificationAsync(DataSpecification dataSpecification, UserMessage userMessage)
	{
		List<DataSpecificationItemMapping> mappings = await _llmConnector.MapUserMessageToDataSpecificationAsync(dataSpecification, userMessage);
		StoreMappingsAndUpdateItsReferences(mappings);
		return mappings;
	}

	private async Task<List<DataSpecificationItemMapping>> MapToSubstructureAsync(DataSpecification dataSpecification, DataSpecificationSubstructure substructure, UserMessage userMessage)
	{
		// The substructure should already contains all data specification items.
		// This method will only map words from the userMessage to items in the substructure.
		List<DataSpecificationItemMapping> mappings = await _llmConnector.MapUserMessageToSubstructureAsync(dataSpecification, substructure, userMessage);
		StoreMappingsAndUpdateItsReferences(mappings);
		return mappings;
	}

	private async Task<List<DataSpecificationItemSuggestion>> GetSuggestionsAsync(DataSpecification dataSpecification, DataSpecificationSubstructure substructure, UserMessage userMessage)
	{
		List<DataSpecificationItemSuggestion> suggestedProperties = await _llmConnector.GetSuggestedPropertiesAsync(
				userMessage.Conversation.DataSpecification, substructure, userMessage);
		if (suggestedProperties.Count == 0)
		{
			return [];
		}

		foreach (DataSpecificationItemSuggestion suggestion in suggestedProperties)
		{
			// Check the databas for the item.
			DataSpecificationItem? property = _database.DataSpecificationItems
				.SingleOrDefault(item => item.DataSpecificationId == suggestion.ItemDataSpecificationId && item.Iri == suggestion.ItemIri);
			DataSpecificationItem? domain = _database.DataSpecificationItems
				.SingleOrDefault(item => item.DataSpecificationId == suggestion.ItemDataSpecificationId && item.Iri == suggestion.DomainItemIri);

			// Change the references to the items from the database.
			// So that there is no duplicate item conflict when I save later.
			if (property is not null)
			{
				suggestion.Item = property;
			}
			if (domain is not null)
			{
				suggestion.DomainItem = domain;
			}
			else
			{
				await _database.DataSpecificationItems.AddAsync(suggestion.DomainItem);
				await _database.SaveChangesAsync();
			}

			if (suggestion.Item.Type is ItemType.ObjectProperty)
			{
				DataSpecificationItem? range = _database.DataSpecificationItems
					.SingleOrDefault(item => item.DataSpecificationId == suggestion.ItemDataSpecificationId && item.Iri == suggestion.RangeItemIri);
				if (range is not null)
				{
					suggestion.RangeItem = range;
				}
				else
				{
					if (suggestion.RangeItem is null)
					{
						_logger.LogError("Suggestion is an ObjectProperty but RangeItem is null.");
					}
					else
					{
						await _database.DataSpecificationItems.AddAsync(suggestion.RangeItem);
						await _database.SaveChangesAsync();
					}
				}
			}

			// Add the information for the front end: which item in the current substructure is expanded by this suggestion.
			if (substructure.ClassItems.Any(c => c.Iri == suggestion.DomainItem.Iri))
			{
				suggestion.ExpandsItem = suggestion.DomainItem.Iri;
			}
			else if (substructure.ClassItems.Any(c => c.Iri == suggestion.RangeItemIri))
			{
				suggestion.ExpandsItem = suggestion.RangeItemIri;
			}

			suggestion.Item.ItemSuggestionsTable.Add(suggestion);
			suggestion.ReplyMessage.ItemSuggestions.Add(suggestion);
		}
		return suggestedProperties;
	}

	// To do: Methods AddMappedItemsToSubstructure and AddSelectedItemsToSubstructure could probably be somehow compressed into 1 method. There are duplicated parts.
	// The only difference is the add mappings has one additional info: whether or not the class is select target.

	private void AddMappedItemsToSubstructure(DataSpecificationSubstructure substructure, IReadOnlyCollection<DataSpecificationItemMapping> mappings)
	{
		// Add all classes to the substructure.
		IEnumerable<DataSpecificationItemMapping> classMappings = mappings.Where(m => m.Item.Type is ItemType.Class);
		foreach (var classMapping in classMappings)
		{
			if (substructure.ClassItems.Any(c => c.Iri == classMapping.Item.Iri))
			{
				// This should never happen.
				// But check just in case.
				_logger.LogWarning("Class \"{Label}\" is already in the substructure.", classMapping.Item.Label);
				continue;
			}
			else
			{
				DataSpecificationSubstructure.ClassItem classItem = new()
				{
					Iri = classMapping.Item.Iri,
					Label = classMapping.Item.Label,
					IsSelectTarget = classMapping.IsSelectTarget
				};
				substructure.ClassItems.Add(classItem);
			}
		}

		// Add properties to the substructure.
		IEnumerable<DataSpecificationItem> mappedProperties = mappings
																	.Where(m => m.Item.Type is ItemType.ObjectProperty || m.Item.Type is ItemType.DatatypeProperty)
																	.Select(m => m.Item);
		foreach (DataSpecificationItem property in mappedProperties)
		{
			if (property.DomainItemIri is null || property.RangeItemIri is null)
			{
				_logger.LogError("Property domain or range is null. Domain: {Domain}, Range: {Range}", property.DomainItemIri, property.RangeItemIri);
				continue;
			}
			if (property.Type is ItemType.ObjectProperty && !substructure.ClassItems.Any(c => c.Iri == property.RangeItemIri))
			{
				_logger.LogWarning("The property {PropLabel} does not have its range in the substructure.", property.Label);
			}

			DataSpecificationSubstructure.ClassItem? domainClass = substructure.ClassItems.Find(c => c.Iri == property.DomainItemIri);
			if (domainClass is null)
			{
				_logger.LogError("The property {PropLabel} does not have its domain in the substructure.", property.Label);
				continue;
			}

			DataSpecificationSubstructure.PropertyItem propertyItem = new()
			{
				Iri = property.Iri,
				Label = property.Label,
				Domain = property.DomainItemIri,
				Range = property.RangeItemIri
			};

			switch (property.Type)
			{
				case ItemType.Class:
					_logger.LogError("Expected only properties in the mappedProperties collection. Found a class instead.");
					break;
				case ItemType.ObjectProperty:
					domainClass.ObjectProperties.Add(propertyItem);
					break;
				case ItemType.DatatypeProperty:
					domainClass.DatatypeProperties.Add(propertyItem);
					break;
			}
		}
	}

	private void AddSelectedItemsToSubstructure(DataSpecificationSubstructure substructure, IReadOnlyCollection<DataSpecificationItem> itemsToAdd)
	{
		// Add all classes to the substructure.
		IEnumerable<DataSpecificationItem> classesToAdd = itemsToAdd.Where(item => item.Type is ItemType.Class);
		foreach (var classToAdd in classesToAdd)
		{
			if (substructure.ClassItems.Any(c => c.Iri == classToAdd.Iri))
			{
				// This shouldn't happen because I already filtered the selected items when user selects items to add.
				// But checking it again just in case.
				_logger.LogWarning("Class \"{Label}\" is already in the substructure.", classToAdd.Label);
				continue;
			}
			else
			{
				DataSpecificationSubstructure.ClassItem classItem = new()
				{
					Iri = classToAdd.Iri,
					Label = classToAdd.Label,
					IsSelectTarget = false // I have left this for future work. In this version, add all user selected items as non-select target.
				};
				substructure.ClassItems.Add(classItem);
			}
		}

		// Add properties to the substructure.
		IEnumerable<DataSpecificationItem> mappedProperties = itemsToAdd
																	.Where(item => item.Type is ItemType.ObjectProperty || item.Type is ItemType.DatatypeProperty);
		foreach (DataSpecificationItem property in mappedProperties)
		{
			if (property.DomainItemIri is null || property.RangeItemIri is null)
			{
				_logger.LogError("Property domain or range is null. Domain: {Domain}, Range: {Range}", property.DomainItemIri, property.RangeItemIri);
				continue;
			}
			if (property.Type is ItemType.ObjectProperty && !substructure.ClassItems.Any(c => c.Iri == property.RangeItemIri))
			{
				_logger.LogWarning("The property {PropLabel} does not have its range in the substructure.", property.Label);
			}

			DataSpecificationSubstructure.ClassItem? domainClass = substructure.ClassItems.Find(c => c.Iri == property.DomainItemIri);
			if (domainClass is null)
			{
				_logger.LogError("The property {PropLabel} does not have its domain in the substructure.", property.Label);
				continue;
			}

			DataSpecificationSubstructure.PropertyItem propertyItem = new()
			{
				Iri = property.Iri,
				Label = property.Label,
				Domain = property.DomainItemIri,
				Range = property.RangeItemIri
			};

			switch (property.Type)
			{
				case ItemType.Class:
					_logger.LogError("Expected only properties in the mappedProperties collection. Found a class instead.");
					break;
				case ItemType.ObjectProperty:
					domainClass.ObjectProperties.Add(propertyItem);
					break;
				case ItemType.DatatypeProperty:
					domainClass.DatatypeProperties.Add(propertyItem);
					break;
			}
		}
	}

	#endregion Private methods
}
