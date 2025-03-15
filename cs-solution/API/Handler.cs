using Backend.Abstractions;
using Backend.Implementation.Database;
using Backend.DTO;
using Backend.Implementation;
using Backend.Model;

namespace RequestHandler;
public class Handler
{
	private static IDatabase _database = new InMemoryDatabase();

	public static class POST
	{
		public static uint CreateConversation(PostConversationsRequestDTO postConversationsRequestDTO)
		{
			string[] iriParts = postConversationsRequestDTO.DataSpecificationIri.Split('/');
			// The iri looks like this: /data-specifications/{dataSpecificationId}
			// So iriParts will be: [ "", "data-specification", "{dataSpecificationid" ]
			if (iriParts.Length != 3)
			{
				Console.WriteLine("Cannot extract the data specification's ID from the IRI: {0}", postConversationsRequestDTO.DataSpecificationIri);
				throw new Exception("The data specification IRI is in an unexpected format.");
			}
			uint.TryParse(iriParts[2], out uint dataSpecificationId);

			DataSpecification dataSpecification = _database.GetDataSpecificationById(dataSpecificationId);
			Conversation conversation = new Conversation(dataSpecification, postConversationsRequestDTO.ConversationTitle);
			_database.AddNewConversation(conversation);
			return conversation.Id;
		}
	}
}
