using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class MockLlmConnector : ILlmConnector
{

	static private MockLlmConnector? _instance = null;

	public static MockLlmConnector GetInstance()
	{
		if (_instance == null)
			return new MockLlmConnector();
		else return _instance;
	}

	private MockLlmConnector() { }

	public string SendPromptAndReceiveResponse(string prompt)
	{
		Console.WriteLine("MockLlmConnector::SendPromptAndReceiveResponse() invoked.");
		return "This is a dummy LLM response from the MockLlmConnector.";
	}

	/*public string TranslateToSparql(string prompt)
	{
		return """
				SELECT ?player
				WHERE
				{
					?player participates ?tournament
				}
				""";
	}

	public string GetExpansionProperties(string userQuery)
	{
		return """
			[
				{
					"MappedWord": "players",
					"PropertyInfo": {
						"Name": "participant",
						"Summary": "Each participant is registered with the BWF...."
					}
				},
				{
					"MappedWord": "tournament",
					"PropertyInfo": {
						"Name": "badminton tournament",
						"Summary": "Tournaments are held yearly. Participants earn points towards the World Tour...."
					}
				}
			]
			""";
	}

	public string GetPropertySummary(DataSpecification dataSpecification, uint propertyId)
	{
		return "These are the players or teams that are taking part in the badminton tournament. Each participant can be an individual player or a group of players (like a team), and they represent different countries.";
	}*/
}
