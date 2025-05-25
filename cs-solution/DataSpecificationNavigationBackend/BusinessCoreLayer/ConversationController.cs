using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IConversationService""/> to HTTP responses.
/// </summary>
public class ConversationController(
	IConversationService conversationService) : IConversationController
{
	private readonly IConversationService _conversationService = conversationService;

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
		Conversation? conversation = _conversationService.GetConversation(conversationId);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		return Results.Ok(conversation.Messages);
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
		//Message userMessage = new(MessageType.UserMessage, 69, payload.TextValue, payload.TimeStamp);

		// Some LLM shaenanigans needed here.
		Message userMessage = _conversationService.AddUserMessage(conversationId, payload.TimeStamp, payload.TextValue);
		return Results.Created($"/conversations/{conversationId}/messages/{userMessage.Id}", userMessage);
	}
}
