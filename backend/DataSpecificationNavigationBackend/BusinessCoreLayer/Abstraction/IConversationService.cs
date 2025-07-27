using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	Task<Conversation> StartNewConversationAsync(string conversationTitle, DataSpecification dataSpecification);

	Task<IReadOnlyList<Conversation>> GetAllConversationsAsync();

	Task<Conversation?> GetConversationAsync(int conversationId, bool includeMessages = false);

	Task<Message> AddNewUserMessage(Conversation conversation, string messageText, DateTime timestamp, bool userModifiedSuggestedMessage);

	/// <summary>
	/// Generate a reply to the user's message.
	/// </summary>
	/// <param name="userMessage">The message to reply to.</param>
	/// <returns>
	/// The reply message containing the Sparql query and related items.
	/// </returns>
	Task<Message?> GenerateReplyMessage(Message userMessage);
}
