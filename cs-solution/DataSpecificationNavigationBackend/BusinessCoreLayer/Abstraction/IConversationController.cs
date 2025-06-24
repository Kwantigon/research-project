using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IConversationController
{
	IResult StartConversation(PostConversationsDTO payload);
	IResult GetOngoingConversations();
	IResult GetConversation(int conversationId);
	IResult GetConversationMessages(int conversationId);
	IResult GetMessage(int conversationId, int messageId);
	IResult ProcessUserMessage(int conversationId, PostConversationMessagesDTO payload);
}
