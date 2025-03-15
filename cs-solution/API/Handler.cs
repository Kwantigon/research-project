using Backend.Abstractions;
using Backend.Implementation.Database;
using Backend.DTO;
using Backend.Implementation;
using Backend.Model;

namespace RequestHandler;
public class Handler
{
	private static IDatabase database = new InMemoryDatabase();

	private static ILlmConnector llmConnector = MockLlmConnector.GetInstance();

	private static ILlmResponseProcessor llmResponseProcessor = MockLlmResponseProcessor.GetInstance();

	public static class GET
	{
		public static IList<GetDataSpecificationsResponseDTO> AllDataSpecifications()
		{
			var dataSpecifications = database.GetAllDataSpecifications();
			return dataSpecifications.Select(
				dataSpec => new GetDataSpecificationsResponseDTO()
				{
					Name = dataSpec.Name,
					Location = "/data-specifications/" + dataSpec.Id,
					DataspecerUri = dataSpec.DataspecerUri,
				}
			).ToList();
		}

		public static DataSpecification DataSpecification(uint dataSpecificationId)
		{
			return database.GetDataSpecificationById(dataSpecificationId);
		}

		public static IList<GetConversationsResponseDTO> AllConversations()
		{
			var conversations = database.GetAllConversations();
			return conversations.Select(
				c => new GetConversationsResponseDTO()
				{
					Title = c.Title,
					Location = "/conversations/" + c.Id
				}
			).ToList();
		}

		public static IList<Message> ConversationMessages(uint conversationId)
		{
			Conversation conversation = database.GetConversationById(conversationId);
			return conversation.Messages;
		}
	}

	public static class POST
	{
		public static uint CreateConversation(PostConversationsRequestDTO postConversationsRequestDTO)
		{
			string[] uriParts = postConversationsRequestDTO.DataSpecificationUri.Split('/');
			// The uri looks like this: /data-specifications/{dataSpecificationId}
			// So uriParts will be: [ "", "data-specification", "{dataSpecificationid" ]
			if (uriParts.Length != 3)
			{
				Console.WriteLine("Cannot extract the data specification's ID from the URI: {0}", postConversationsRequestDTO.DataSpecificationUri);
				throw new Exception("The data specification URI is in an unexpected format.");
			}
			uint.TryParse(uriParts[2], out uint dataSpecificationId);

			DataSpecification dataSpecification = database.GetDataSpecificationById(dataSpecificationId);
			Conversation conversation = new Conversation(dataSpecification, postConversationsRequestDTO.ConversationTitle);
			database.AddNewConversation(conversation);
			return conversation.Id;
		}

		public static uint ProcessNewDataSpecification(PostDataSpecificationsRequestDTO dataSpecificationInfo)
		{
			// Do some processing of the data specification....

			// Then save it to the database.
			if (dataSpecificationInfo.UriToDataspecer == null)
			{
				throw new Exception("Data specification URI is null");
			}

			DataSpecification dataSpecification = new DataSpecification(dataSpecificationInfo.Name, dataSpecificationInfo.UriToDataspecer);

			database.AddNewDataSpecification(dataSpecification);
			return dataSpecification.Id;
		}

		/*public static SparqlResponse AddNewMessageToConversation(uint conversationId, PostConversationMessageDTO messageDTO)
		{
			if (messageDTO.Source != 1)
			{
				throw new Exception("Message source is not the user. Message source value=" + messageDTO.Source);
			}
			if (string.IsNullOrWhiteSpace(messageDTO.TextValue))
			{
				throw new Exception("Message does not contain any text");
			}

			Message message = new Message
			{
				Source = (MessageSource)messageDTO.Source,
				TimeStamp = messageDTO.TimeStamp,
				TextValue = messageDTO.TextValue,
			};
			Conversation conversation = database.GetConversationById(conversationId);
			conversation.Messages.Add(message);

			string sparql = llmConnector.TranslateToSparql(messageDTO.TextValue);

			// Just highlight some random word in the query.
			string[] userMsg = message.TextValue.Split(' ');
			Random rng = new Random();
			int startingIndex = rng.Next(userMsg.Length);
			HighlightedProperty highlightedProperty = new HighlightedProperty(startingIndex, 1);

			SparqlResponse response = new SparqlResponse()
			{
				SparqlQuery = sparql,
				HighlightedWords = new List<HighlightedProperty>()
			};
			response.HighlightedWords.Add(highlightedProperty);

			return response;
		}*/
	}
}

