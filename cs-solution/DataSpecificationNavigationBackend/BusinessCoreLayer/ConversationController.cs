using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace DataspecNavigationHelper.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IConversationService""/> to HTTP responses.
/// </summary>
public class ConversationController(
	IConversationService conversationService,
	IDataSpecificationService dataSpecificationService,
	AppDbContext appDbContext) : IConversationController
{
	private readonly IConversationService _conversationService = conversationService;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	private readonly AppDbContext _database = appDbContext;

	public IResult StartConversation(PostConversationsDTO payload)
	{
		DataSpecification? dataSpecification = _dataSpecificationService.GetDataSpecificationByIri(payload.DataSpecificationIri);
		if (dataSpecification == null)
		{
			return Results.NotFound(new Error { Reason = $"Data specification with IRI {payload.DataSpecificationIri} not found." });
		}

		Conversation conversation = _conversationService.StartNewConversation(payload.ConversationTitle, dataSpecification);
		return Results.Ok((ConversationDTO)conversation);
	}

	public IResult GetOngoingConversations()
	{
		/*IReadOnlyList<Conversation> conversations = _conversationService.GetOngoingConversations();
		return Results.Ok(
			conversations.Select(conversation => (ConversationDTO)conversation)
		);*/

		// Mock conversations
		List<ConversationDTO> conversations = [
			new ConversationDTO(1, "Conversation one", "specification one", DateTime.Now.AddDays(-2)),
			new ConversationDTO(2, "Mock convo 2", "Some data spec 2", DateTime.Now.AddDays(-1)),
			new ConversationDTO(3, "Convo three", "Data specification three", DateTime.Now)
		];
		return Results.Ok(conversations);
	}

	public IResult GetConversation(int conversationId)
	{
		Conversation? conversation = _conversationService.GetConversation(conversationId);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}
		
		return Results.Ok((ConversationDTO)conversation);
	}

	public IResult GetConversationMessages(int conversationId)
	{
		/*Conversation? conversation = _conversationService.GetConversation(conversationId);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		return Results.Ok(conversation.Messages);*/

		// Mock messages
		List<Message> messages = [
			new Message()
			{
				Id = 0,
				Type = MessageType.WelcomeMessage,
				TextValue = "Mock welcome message",
				TimeStamp = DateTime.Now
			},
			new Message()
			{
				Id = 1,
				Type = MessageType.UserMessage,
				TextValue = "Hello there (from user)",
				TimeStamp = DateTime.Now
			},
			new Message()
			{
				Id = 2,
				Type = MessageType.ReplyMessage,
				TextValue = "Hi (reply from system)",
				TimeStamp = DateTime.Now,
				RelatedItems = [
					new DataSpecificationItem("http://example.com/class-item", "item of type class", ItemType.Class, "Mock summary for class item"),
					new DataSpecificationItem("http://example.com/objectProperty-item", "item of type objectProperty", ItemType.ObjectProperty),
					new DataSpecificationItem("http://example.com/datatypeProperty-item", "item of type datatypeProperty", ItemType.DatatypeProperty)
				]
			},
			new Message()
			{
				Id = 3,
				Type = MessageType.ReplyMessage,
				TextValue = "Hi (reply from system)",
				TimeStamp = DateTime.Now,
				RelatedItems = [
					new DataSpecificationItem("http://example.com/ab", "item of type class", ItemType.Class, "Mock summary for class item"),
					new DataSpecificationItem("http://example.com/cc", "item of type objectProperty", ItemType.ObjectProperty),
					new DataSpecificationItem("http://example.com/de", "item of type datatypeProperty", ItemType.DatatypeProperty)
				]
			}
		];
		Console.WriteLine("Returning messages.");
		return Results.Ok(messages);
	}

	public IResult GetMessage(int conversationId, int messageId)
	{
		Conversation? conversation = _conversationService.GetConversation(conversationId);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		Message? message = conversation.Messages.Find(msg => msg.Id == messageId);
		if (message == null)
		{
			return Results.NotFound(new Error { Reason = $"Message with ID {messageId} not found." });
		}

		return Results.Ok(message);
	}

	public IResult ProcessUserMessage(int conversationId, PostConversationMessagesDTO payload)
	{
		Message userMessage = _conversationService.AddUserMessage(conversationId, payload.TimeStamp, payload.TextValue);

		// Map the user's question to items from the data specification.

		// Get a list of data specification items that are relevant to the question.

		// Translate to Sparql.

		// Create a reply message and add that message to the conversation. The reply message also contains the list of relevant items.

		return Results.Created($"/conversations/{conversationId}/messages/{userMessage.Id}", userMessage);
		// Todo: Return "replyUri" as part of the answer so that the front end knows, what URI to query for the reply.
	}

	public IResult StartEfTestConversation(PostConversationsDTO payload)
	{
		DataSpecification ds = _database.DataSpecifications.Single(spec => spec.Iri.Equals(payload.DataSpecificationIri));
		if (ds is null)
		{
			ds = new DataSpecification()
			{
				Iri = $"some-iri-{DateTime.Now.ToString()}",
				Name = "mock",
				Owl = "owl placeholder"
			};
			_database.DataSpecifications.Add(ds);
		}

		Conversation conversation = new()
		{
			DataSpecification = ds,
			Messages = [],
			Title = "mock convo",
			LastUpdated = DateTime.Now
		};
		_database.Conversations.Add(conversation);
		_database.SaveChanges();
		return Results.Created($"/ef-test/conversations/{conversation.Id}", conversation);
	}

	public IResult AddEfTestDataSpecification(PostDataSpecificationsDTO payload)
	{
		DataSpecification ds = new DataSpecification()
		{
			Iri = payload.DataspecerPackageIri,
			Name = payload.Name != null ? payload.Name : "Unnamed data specification",
			Owl = "owl placeholder"
		};
		_database.DataSpecifications.Add(ds);
		_database.SaveChanges();
		return Results.Created($"/ef-test/data-specifications/{ds.Id}", ds);
	}
}
