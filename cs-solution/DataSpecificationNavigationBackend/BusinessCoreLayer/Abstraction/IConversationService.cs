using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IConversationService
{
	Conversation StartNewConversation(string? conversationTitle, DataSpecification dataSpecification);
	IReadOnlyList<Conversation> GetOngoingConversations();
	Conversation? GetConversation(int conversationId);
	Message AddUserMessage(int conversationId, DateTime timeStamps, string message);
}
