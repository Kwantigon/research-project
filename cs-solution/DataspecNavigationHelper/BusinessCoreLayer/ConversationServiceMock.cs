using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class ConversationServiceMock : IConversationService
{
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

	public string GetSuggestedMessge(IReadOnlyList<DataSpecificationItem> selectedItems)
	{

	}
}
