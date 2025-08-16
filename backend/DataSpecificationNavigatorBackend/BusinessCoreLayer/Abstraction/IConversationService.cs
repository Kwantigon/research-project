using DataSpecificationNavigatorBackend.Model;

namespace DataSpecificationNavigatorBackend.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification);

	Task<IReadOnlyList<Conversation>> GetAllConversationsAsync();

	Task<Conversation?> GetConversationAsync(int conversationId);

	Task<UserMessage> AddUserMessageAndGenerateReplyAsync(Conversation conversation, string messageText, DateTime timestamp);

	Task<ReplyMessage?> GenerateReplyMessageAsync(UserMessage userMessage);

	Task<string?> UpdateSelectedItemsAndGenerateSuggestedMessageAsync(Conversation conversation, HashSet<string> selectedItems);

	Task<bool> DeleteConversationAndAssociatedDataSpecificationAsync(int conversationId);
}
