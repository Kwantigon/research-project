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

		if (conversation.SuggestedMessage == null || userMessage.TextContent.ToLower() != conversation.SuggestedMessage.ToLower())
		{
			_logger.LogTrace("User has modified the suggested message (or this is the first user message in the conversation).");

			_logger.LogTrace("Mapping the question to data specification items.");
			List<DataSpecificationItemMapping> mappings = await MapToDataSpecificationAsync(conversation.DataSpecification, userMessage);

			_logger.LogTrace("Clearing the data specification substructure of the conversation.");
			conversation.DataSpecificationSubstructure.Clear();
			if (mappings.Count == 0)
			{
				_logger.LogError("No suitable data specification items found for the question mapping.");
				_logger.LogTrace("No mapping found so clearing the conversation data specification substructure.");
				conversation.DataSpecificationSubstructure.Clear();
			}
			else // mappings.Count > 0
			{
				_logger.LogTrace("Setting the conversation data spec substructure and mappings in data spec item and message.");
				StoreMappingsAndUpdateItsReferences(mappings);
				conversation.DataSpecificationSubstructure = mappings.Select(m => m.Item).ToList();
			}
		}
		else // User sent the suggested message as is, without any modifications.
		{
			_logger.LogTrace("User did not modify the suggested message.");

			if (conversation.UserSelectedItems is null || conversation.UserSelectedItems.Count == 0)
			{
				_logger.LogError("userModifiedSuggestedMessage==false but there are no items for expansion selected by the user in the conversation.");
			}
			else
			{
				_logger.LogTrace("Searching for the items that the user has previously selected.");
				List<DataSpecificationItem> selectedItems = await _dataSpecificationService.GetItemsByIriListAsync(conversation.DataSpecification.Id, conversation.UserSelectedItems);
				_logger.LogDebug("conversation.UserSelectedItems.Count = {SelectedCount}, selectedItems.Count = {SelectedFound}", conversation.UserSelectedItems.Count, selectedItems.Count);

				_logger.LogTrace("Filtering the selected items - keeping only those that are not already in the conversation data spec substructure.");
				List<DataSpecificationItem> itemsNotInConversation = selectedItems.Where(selected => !conversation.DataSpecificationSubstructure.Any(i => i.Iri == selected.Iri)).ToList();

				_logger.LogTrace("Adding the selected items to the conversation.");
				conversation.DataSpecificationSubstructure.AddRange(itemsNotInConversation);

				// Do the mapping for items.
				//List<DataSpecificationItemMapping> mappings = await _llmConnector.MapUserMessageToConversationDataSpecSubstructureAsync(userMessage);
				List<DataSpecificationItemMapping> mappings = await MapToSubstructureAsync(userMessage);
				StoreMappingsAndUpdateItsReferences(mappings);
			}
		}

		conversation.UserSelectedItems?.Clear();
		conversation.SuggestedMessage = null;
		_logger.LogTrace("Saving changes to the database and returning.");
		await _database.SaveChangesAsync();
		PrintSubstructureForDebugging(conversation);
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
			// In that case the conversation data specification substructure corresponds to the items mapped from the user message.
			_logger.LogTrace("Generating a Sparql query.");
			replyMessage.SparqlQuery = _sparqlTranslationService.TranslateSubstructure(userMessage.Conversation.DataSpecificationSubstructure);

			_logger.LogTrace("Getting item suggestions.");
			/*List<DataSpecificationItemSuggestion> suggestedItems = await _llmConnector.GetSuggestedItemsAsync(
				userMessage.Conversation.DataSpecification, userMessage, userMessage.Conversation.DataSpecificationSubstructure);*/
			List<DataSpecificationItemSuggestion> suggestedItems = await GetItemSuggestionsAsync(
				userMessage.Conversation.DataSpecification, userMessage, userMessage.Conversation.DataSpecificationSubstructure);
			_logger.LogTrace("The LLM suggested {ItemsCount} items.", suggestedItems.Count);

			if (suggestedItems.Count == 0)
			{
				replyMessage.SuggestItemsText = "Unfortunately, I did not manage to find any suitable items to suggest to you to further expand your question.";
			}
			else
			{
				replyMessage.SuggestItemsText = "I found some items which could expand your question.";

				foreach (DataSpecificationItemSuggestion suggestion in suggestedItems)
				{
					// Check the database and conversation substructure for the item.
					DataSpecificationItem? item = await _database.DataSpecificationItems
						.SingleOrDefaultAsync(item => item.DataSpecificationId == suggestion.ItemDataSpecificationId && item.Iri == suggestion.ItemIri);
					if (item is not null)
					{
						// Change the reference to the actual item from the database.
						// So that there is no duplicate item conflict when I save later.
						suggestion.Item = item;
					}
					suggestion.Item.ItemSuggestionsTable.Add(suggestion);
					suggestion.ReplyMessage.ItemSuggestions.Add(suggestion);
				}
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

		conversation.UserSelectedItems = selectedItems.Select(item => item.Iri).ToList();
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

	}

	private async Task<List<DataSpecificationItemMapping>> MapToSubstructureAsync(DataSpecificationSubstructure substructure, UserMessage userMessage)
	{
		// The substructure should already contains all data specification items.
		// This method will only map words from the userMessage to items in the substructure.
	}

	private async Task<List<DataSpecificationItemSuggestion>> GetItemSuggestionsAsync(DataSpecification dataSpecification, UserMessage userMessage, IReadOnlyCollection<DataSpecificationItem> dataSpecificationSubstructure)
	{

	}

	private void AddSelectedItemsToSubstructure(DataSpecificationSubstructure substructure, IReadOnlyCollection<DataSpecificationItem> itemsToAdd)
	{
		// itemsToAdd obsahuje třídy a properties těch tříd.
		// Musím poskládat, jaké properties patří jaké třídě.
		// Pak jen stačí nacpat buď do substructure.Targets nebo substructure.NonTargets.

		var allSubstructureClasses = substructure.Targets.Concat(substructure.NonTargets).ToList();
		// Key = class item IRI, Value = the class item object.
		Dictionary<string, DataSpecificationSubstructure.ClassItem> map = allSubstructureClasses.ToDictionary(c => c.Iri, c => c);
		foreach (DataSpecificationItem item in itemsToAdd)
		{
			switch (item.Type)
			{
				case ItemType.Class:
					if (map.ContainsKey(item.Iri))
					{
						// This case should not happen.
						// I should only be adding items that are not already in the substructure.
						_logger.LogWarning("Item {Label} is already in the data specification substructure.", item.Label);
					}
					else
					{
						DataSpecificationSubstructure.ClassItem newItem = new();
						newItem.Iri = item.Iri;
						newItem.Label = item.Label;
						// To do: Add to targets or non-targets?
						substructure.Targets.Add(newItem);
						//substructure.NonTargets.Add(newItem);
						map.Add(item.Iri, newItem);
					}
					break;
				case ItemType.ObjectProperty:
					if (item.Domain is null || item.Range is null)
					{
						_logger.LogError("Item {Label} is of type ObjectProperty but either domain or range is null.", item.Label);
						throw new Exception("ObjectProperty has null domain or range.");
					}
					DataSpecificationSubstructure.ClassItem? domain = map[item.Domain];
					DataSpecificationSubstructure.ClassItem? range = map[item.Range];
					if (domain is null || range is null)
					{
						// It could be that the null value is not yet added.
						// This will likely happen often with range.
						// I'm adding a new property and its range is another new class that is in 'itemsToAdd'
						// but I haven't gotten to that item yet.
						//
						// So I should first add all classes and then add properties.
						// And I should make sure that whenever the substructure is expanded,
						// it is expanded with both the property and its range or domain.
					}
					break;
				case ItemType.DatatypeProperty:
					break;
			}
		}
	}

	private void PrintSubstructureForDebugging(Conversation conversation)
	{
		StringBuilder sb = new();
		foreach (var item in conversation.DataSpecificationSubstructure)
		{
			sb.AppendLine($"- {item.Label} ({item.Iri})");
		}
		_logger.LogDebug("Current data specification substructure in the conversation:\n{Substructure}", sb.ToString());
	}

	#endregion Private methods
}
