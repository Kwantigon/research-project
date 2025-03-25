using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class ConversationService : IConversationService
{
	public UserMessage AddAndReturnUserMessageToConversation(Conversation conversation, string messageText, DateTime messageTimeStamp)
	{
		UserMessage userMessage = new UserMessage { Id = conversation.NextUnusedMessageId++, TextValue = messageText, TimeStamp = messageTimeStamp };
		conversation.Messages.Add(userMessage);
		conversation.State = ConversationState.AwaitingFollowUpUserMessage;
		return userMessage;
	}

	public void AddItemsToSubstructurePreview(Conversation conversation, IReadOnlyList<DataSpecificationItem> itemsToAdd)
	{
		conversation.NextQuestionSubstructurePreview = new DataSpecificationSubstructure();
	}

	public DataSpecificationSubstructure? ConfirmSubstructurePreview(Conversation conversation)
	{
		conversation.DataSpecificationSubstructure = conversation.NextQuestionSubstructurePreview;
		conversation.NextQuestionSubstructurePreview = null;
		return conversation.DataSpecificationSubstructure;
	}

	public DataSpecificationSubstructure? CreateDataSpecificationSubstructureForConversation(Conversation conversation, IReadOnlyList<DataSpecificationItem> dataSpecificationItems)
	{
		throw new NotImplementedException();
	}

	public Conversation CreateNewConversation(DataSpecification dataSpecification, string? conversationTitle = null)
	{
		Conversation conversation = new(dataSpecification, conversationTitle);
		conversation.InitializeConversation();
		return conversation;
	}

	public void UpdateQuestionPreview(Conversation conversation, string questionText)
	{
		throw new NotImplementedException();
	}
}
