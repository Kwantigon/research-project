using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class ConversationServiceMock : IConversationService
{
	public IReadOnlyList<Conversation> GetOngoingConversations()
	{
		return new List<Conversation>
		{
			new Conversation { Id = 1, Title = "Mock conversation 1" },
			new Conversation { Id = 2, Title = "Mock conversation 2" },
		};
	}

	public Conversation? GetConversation(int conversationId)
	{
		if (conversationId < 0)
		{
			// Simulate the situation of ID not found in the database.
			return null;
		} else
		{
			Conversation conversation = new() { Id = conversationId, Title = "Mock conversation" };
			conversation.Messages.Add(new Message(MessageType.WelcomeMessage, 0, "Mock welcome message", DateTime.Now));
			conversation.Messages.Add(new Message(MessageType.UserMessage, 1, "Mock user message", DateTime.Now.AddSeconds(5)));
			return conversation;
		}
	}

	public string GetReply(Message userMessage)
	{
		if (userMessage.TextValue == "Your data specification has been loaded. What would you like to know?")
		{
			return """
				The data you want can be retrieved using the following Sparql query
				SELECT DISTINCT ?tournament
				WHERE {
					?tournament http://example.com/tournaments/date-held http://example.com/years:2025
				}
				""";
		}

		return "I could not find any suitable matches from the data specification.";
	}

	public Message AddUserMessage(int conversationId, DateTime timeStamps, string message)
	{
		throw new NotImplementedException();
	}
}
