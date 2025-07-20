using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification);

	Task<IReadOnlyList<Conversation>> GetAllConversationsAsync();

	Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false);

	Task<Message> AddNewUserMessage(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage);
}
