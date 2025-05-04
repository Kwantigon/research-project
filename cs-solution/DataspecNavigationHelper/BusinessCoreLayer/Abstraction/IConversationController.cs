namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IConversationController
{
	IResult GetOngoingConversations();
	IResult GetConversation(int conversationId);
	IResult GetConversationMessages(int conversationId);
	IResult GetMessage(int conversationId, int messageId);
}
