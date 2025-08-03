using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification);

	Task<IReadOnlyList<Conversation>> GetAllConversationsAsync();

	Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false);

	Task<UserMessage> AddNewUserMessageAsync(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage);

	Task<ReplyMessage?> GenerateReplyMessageAsync(UserMessage userMessage);

	Task<string?> UpdateSelectedItemsAndGenerateSuggestedMessageAsync(Conversation conversation, List<DataSpecificationItem> selectedItems);

	Task<bool> DeleteConversationAndAssociatedResourcesAsync(int conversationId);
}
