using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IConversationController
{
	Task<IResult> StartConversation(PostConversationsDTO payload);
	Task<IResult> GetOngoingConversations();
	IResult GetConversation(int conversationId);
	IResult GetConversationMessages(int conversationId);
	IResult GetMessage(int conversationId, int messageId);
	IResult ProcessUserMessage(int conversationId, PostConversationMessagesDTO payload);
	Task<IResult> DeleteConversation(int conversationId);

	IResult StartEfTestConversation(PostConversationsDTO payload);
}
