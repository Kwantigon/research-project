using DataSpecificationNavigatorBackend.BusinessCoreLayer.DTO;

namespace DataSpecificationNavigatorBackend.BusinessCoreLayer.Abstraction;

public interface IConversationController
{
	Task<IResult> StartConversationAsync(PostConversationsDTO payload);
	Task<IResult> GetAllConversationsAsync();

	Task<IResult> GetConversationAsync(int conversationId);

	/// <summary>
	/// Get messages in a conversation.
	/// </summary>
	/// <param name="conversationId">The ID of the conversation.</param>
	/// <returns>All messages in the conversation ordered by their timestamps.</returns>
	Task<IResult> GetConversationMessagesAsync(int conversationId);

	Task<IResult> GetMessageAsync(int conversationId, Guid messageId);

	Task<IResult> ProcessIncomingUserMessage(int conversationId, PostConversationMessagesDTO payload);

	Task<IResult> DeleteConversationAsync(int conversationId);

	Task<IResult> StoreUserSelectionAndGetSuggestedMessage(int conversationId, PutDataSpecItemsDTO payload);
}
