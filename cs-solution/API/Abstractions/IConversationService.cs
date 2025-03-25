using Backend.Model;

namespace Backend.Abstractions;

public interface IConversationService
{
	Conversation CreateNewConversation(DataSpecification dataSpecification, string? conversationTitle = null);

	UserMessage AddAndReturnUserMessageToConversation(Conversation conversation, string messageText, DateTime messageTimeStamp);

	DataSpecificationSubstructure? CreateDataSpecificationSubstructureForConversation(Conversation conversation, IReadOnlyList<DataSpecificationItem> dataSpecificationItems);

	DataSpecificationSubstructure? ConfirmSubstructurePreview(Conversation conversation);

	void AddItemsToSubstructurePreview(Conversation conversation, IReadOnlyList<DataSpecificationItem> itemsToAdd);

	void UpdateQuestionPreview(Conversation conversation, string questionText);
}
