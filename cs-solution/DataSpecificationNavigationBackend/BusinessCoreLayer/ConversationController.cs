using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.Model;
using System.Collections;

namespace DataspecNavigationHelper.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IConversationService""/> to HTTP responses.
/// </summary>
public class ConversationController(
	IConversationService conversationService,
	IDataSpecificationService dataSpecificationService) : IConversationController
{
	private readonly IConversationService _conversationService = conversationService;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;

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
		IReadOnlyList<Conversation> conversations = _conversationService.GetOngoingConversations();
		return Results.Ok(
			conversations.Select(conversation => (ConversationDTO)conversation)
		);
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
				TimeStamp = DateTime.Now
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
	}
}
