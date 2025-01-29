using BackendApi.Abstractions;
using BackendApi.Database;
using BackendApi.DTO;
using BackendApi.Implementation;
using BackendApi.Model;

namespace RequestHandler;
public class Handler
{
	private static IDatabase database = new InMemoryDatabase();

	private static ILlmConnector llmConnector = MockLlmConnector.GetInstance();

	private static ILlmResponseProcessor llmResponseProcessor;

	public static class GET
	{
		public static IList<GetDataSpecificationsResponseDTO> AllDataSpecifications()
		{
			var dataSpecifications = database.GetAllDataSpecifications();
			return dataSpecifications.Select(
				dataSpec => new GetDataSpecificationsResponseDTO()
				{
					Id = dataSpec.Id,
					Name = dataSpec.Name
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
					ConversationId = c.Id,
					Title = c.Title
				}
			).ToList();
		}

		public static IList<Message> ConversationMessages(uint conversationId)
		{
			Conversation conversation = database.GetConversationById(conversationId);
			return conversation.Messages;
		}

		public static PropertySummary PropertySummary(uint dataSpecificationId, uint propertyId)
		{
			DataSpecification dataSpecification = database.GetDataSpecificationById(dataSpecificationId);
			string summary = llmConnector.GetPropertySummary(dataSpecification, propertyId);
			return llmResponseProcessor.ProcessPropertySummaryResponse(summary);
		}
	}

	public static class POST
	{
		public static uint CreateConversation(PostConversationsRequestDTO postConversationsRequestDTO)
		{
			DataSpecification dataSpecification = database.GetDataSpecificationById(postConversationsRequestDTO.DataSpecificationId);
			Conversation conversation = new Conversation(dataSpecification, postConversationsRequestDTO.ConversationTitle);
			database.AddNewConversation(conversation);
			return conversation.Id;
		}

		public static uint ProcessNewDataSpecification(PostDataSpecificationsRequestDTO dataSpecificationInfo)
		{
			// Do some processing of the data specification....

			// Then save it to the database.
			if (dataSpecificationInfo.Uri == null)
			{
				throw new Exception("Data specification URI is null");
			}

			DataSpecification dataSpecification = new DataSpecification(dataSpecificationInfo.Name, dataSpecificationInfo.Uri);

			database.AddNewDataSpecification(dataSpecification);
			return dataSpecification.Id;
		}

		/*public static void AddBotMessageToConversation(uint conversationId, PostConversationMessageDTO messageDTO)
		{
			if (messageDTO.Source != 0)
			{
				throw new Exception("Message source is not the chatbot. Message source value=" + messageDTO.Source);
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
		}*/

		public static SparqlResponse AddNewMessageToConversation(uint conversationId, PostConversationMessageDTO messageDTO)
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
		}
	}
}

