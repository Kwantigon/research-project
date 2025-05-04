using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	IReadOnlyList<Conversation> GetOngoingConversations();
	Conversation? GetConversation(int conversationId);
}
