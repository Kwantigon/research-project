using DTO;

namespace RequestHandler;
public class Handler
{
	public static class GET
	{
		public static IReadOnlyList<uint> PreviousConversations()
		{
			Console.WriteLine("Handler.Get.PreviousConversations()");

			// Get previous chats from the databse.
			// Get their IDs and put them into a list.
			// Return that list.
			return new List<uint>() { 1, 2, 3, 4 };
		}

		public static List<ChatMessage> ConversationHistory(uint conversationId)
		{
			Console.WriteLine("Handler.Get.ConversationHistory({id})", conversationId);
			return new List<ChatMessage>()
			{
				new ChatMessage("user-message", "Message from the user"),
				new ChatMessage("bot-message", "Reply from the chatbot"),
				new ChatMessage("user-message", "Message from the user"),
				new ChatMessage("bot-message", "Reply from the chatbot"),
				new ChatMessage("user-message", "Message from the user"),
				new ChatMessage("bot-message", "Reply from the chatbot")
			};
		}

		public static IReadOnlyList<string> QueryExpansionProperties(string userQuery)
		{
			Console.WriteLine("Handler.Get.PropertiesForExpansion()");
			Console.WriteLine("User's query: {0}", userQuery);

			return new List<string>() { "property one", "property number two" };
		}

		public static string PropertySummary(string entityName)
		{
			Console.WriteLine("Handler.Get.EntitySummary()");
			return $"{{ \"entityName\": \"{entityName}\", \"summary\": \"<summary_here>\" }}";
		}
	}

	public static class POST
	{
		public static string DataSpecification(DataSpecificationUrl dataSpecUrl)
		{
			Console.WriteLine("Handler.Post.DataSpecification");
			Console.WriteLine("The dataSpecUrl is: {0}", dataSpecUrl);

			// ToDo: Implement
			// Do not forget sanitization of the dataSpecUrl.

			return "Data specification successfully loaded";
		}

		public static string UserInitialQuery(UserQuery initialQuery)
		{
			Console.WriteLine("Received a POST /user-initial-query request.");
			Console.WriteLine("The query is: {0}", initialQuery);

			// ToDo: Implement
			// Do not forget sanitization of the initialQuery.

			string sparqlQuery = $"{{ \"sparql\": \"<sparql_query_here>\" }}";
			return sparqlQuery;
		}

		public static string UserExpandedQuery(UserQuery expandedQuery)
		{
			Console.WriteLine("Received a POST /user-expanded-query request.");
			Console.WriteLine("The query is: {0}", expandedQuery);

			// ToDo: Implement
			// Do not forget sanitization of the expandedQuery.

			string sparqlQuery = $"{{ \"sparql\": \"<sparql_query_here>\" }}";
			return sparqlQuery;
		}
	}

	public static class PUT
	{
		public static string ExpandQuery(ExpandQueryDTO expandQueryDTO)
		{
			Console.WriteLine("Handler.Get.ExpandedQuery()");
			Console.WriteLine("User's query: {0}", expandQueryDTO.QueryToExpand);
			Console.WriteLine("Properties:");
			foreach (string property in expandQueryDTO.Properties)
			{
				Console.WriteLine("  {0}", property);
			}

			return "<expanded_query_value>";
		}
	}
}

