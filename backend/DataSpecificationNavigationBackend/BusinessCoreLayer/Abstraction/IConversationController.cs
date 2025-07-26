using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationBackend.BusinessCoreLayer.DTO;

namespace DataspecNavigationBackend.BusinessCoreLayer.Abstraction;

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

	Task<IResult> ProcessUserMessageAsync(int conversationId, PostConversationMessagesDTO payload);

	Task<IResult> DeleteConversationAsync(int conversationId);

	//IResult StartEfTestConversation(PostConversationsDTO payload);
}
