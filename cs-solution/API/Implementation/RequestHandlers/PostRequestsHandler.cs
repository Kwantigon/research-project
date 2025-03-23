using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;
using System.Text;

namespace Backend.Implementation.RequestHandlers;

public class PostRequestsHandler(ILogger<PostRequestsHandler> logger, IDatabase database) : IPostRequestsHandler
{
	private readonly ILogger<PostRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;

	#region Interface implementation.
	public IResult PostDataSpecifications(PostDataSpecificationsRequestDTO payload)
	{
		if (string.IsNullOrEmpty(payload.IriToDataspecer))
		{
			return Results.BadRequest(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "No IRI to a Dataspecer package was given"
			});
		}

		DataSpecification dataSpecNew = new DataSpecification(payload.Name, payload.IriToDataspecer);
		if (_database.AddNewDataSpecification(dataSpecNew) is false)
		{
			_logger.LogError("Failed to add the data specification {DataSpec} to the database.", dataSpecNew);
			return Results.InternalServerError(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.InternalServerError,
				ErrorMessage = "Failed to create a new data specification"
			});
		}
		else
		{
			return Results.Created(uri: $"/data-specifications/{dataSpecNew.Id}", dataSpecNew);
		}
	}

	public IResult PostConversations(PostConversationsRequestDTO payload)
	{
		if (string.IsNullOrEmpty(payload.DataSpecificationIri))
		{
			return Results.BadRequest(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The data specification was either null or empty"
			});
		}

		string[] iriParts = payload.DataSpecificationIri.Split('/');
		// The iri looks like this: /data-specifications/{dataSpecificationId}
		// So iriParts will be: [ "", "data-specification", "{dataSpecificationid" ]
		if (iriParts.Length != 3)
		{
			_logger.LogError("Data specification IRI has too many \'/\' characters: {0}", payload.DataSpecificationIri);
			return Results.BadRequest(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The data specification's IRI has an unexpected format"
			});
		}
		if (!uint.TryParse(iriParts[2], out uint dataSpecificationId))
		{
			_logger.LogError("Failed to parse {ID} as an uint.", iriParts[2]);
			return Results.BadRequest(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The data specification's ID in the IRI has an invalid format"
			});
		}

		DataSpecification? dataSpecification = _database.GetDataSpecificationById(dataSpecificationId);
		if (dataSpecification is null)
		{
			_logger.LogError("Failed to retrieve the data specification with ID {DataSpecId} from the database.", dataSpecificationId);
			throw new Exception("Failed to find the requested data specification");
		}

		Conversation conversation = new Conversation(dataSpecification, payload.ConversationTitle);
		if (_database.AddNewConversation(conversation) is false)
		{
			_logger.LogError("Failed to add the conversation {Conversation} to the database.", conversation);
			return Results.InternalServerError(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.InternalServerError,
				ErrorMessage = "Failed to start a new conversation"
			});
		}

		conversation.InitializeConversation();
		return Results.Created($"/conversations/{conversation.Id}", (ConversationDTO)conversation);
	}

	// To do: This method should be async.
	public IResult PostConversationMessages(uint conversationId, PostConversationMessagesDTO payload)
	{
		if (string.IsNullOrEmpty(payload.TextValue))
		{
			return Results.BadRequest(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The text in the message was either null or empty"
			});
		}

		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			return Results.NotFound(new ErrorResponseDTO()
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		var userMessage = new UserMessage()
		{
			Id = conversation.NextUnusedMessageId++,
			TimeStamp = payload.TimeStamp,
			TextValue = payload.TextValue
		};
		_conversationService.AddMessageToConversation(conversation, userMessage);

		// Jsou tu 3 moznosti:
		// Jedna se o uplne prvni zpravu od uzivatele
		// Jedna se o preview zpravu, kterou uzivatel jen potvrdil.
		// Jedna se o preview zpravu, kterou uzivatel upravil.
		DataSpecificationSubstructure substructure;
		if (conversation.State is ConversationState.AwaitingFirstUserMessage)
		{
			string itemsMappingPrompt = _promptConstructor.CreateItemsMappingPrompt(userMessage.TextValue);
			string itemsMappingResponse = _llmConnector.SendPrompt(itemsMappingPrompt);
			DataSpecificationItemsFromLlm mappedItems = _llmResponseProcessor.ProcessItemsMappingResponse(itemsMappingResponse);
			// To do: If the response processor fails to parse, it's likely because the LLM returned an invalid answer format.
			// Try to resend the prompt and ask for the correct answer format.

			substructure = _dataSpecificationService.CreateNewSubstructureOrSmthingIdk(mappedItems);
		}
		
		if (conversation.State is ConversationState.AwaitingUserFollowUpMessage)
		{
			if (payload.UserModifiedPreviewMessage)
			{
				// I don't know how much the user has modified.
				// If they completely changed the question, then it has to be processed as if it was the first question.
				// But maybe they only changed a little bit and I can somehow just change the preview substructure a bit?

				// I think for now, just process like the first message.
				string itemsMappingPrompt = _promptConstructor.CreateItemsMappingPrompt(userMessage.TextValue);
				string itemsMappingResponse = _llmConnector.SendPrompt(itemsMappingPrompt);
				DataSpecificationItemsFromLlm mappedItems = _llmResponseProcessor.ProcessItemsMappingResponse(itemsMappingResponse);
				substructure = _dataSpecificationService.CreateNewSubstructureOrSmthingIdk(mappedItems);
			}
			else
			{
				substructure = _dataSpecificationService.ConfirmPreviewSubstructure(conversation);
			}
		}

		uint systemAnswerId = conversation.NextUnusedMessageId++;
		if (substructure is null)
		{
			var systemAnswer = new NegativeSystemAnswer()
			{
				Id = systemAnswerId,
				TimeStamp = DateTime.Now,
				TextValue = "Sorry, I could not come up with an answer to your question." // To do: This string should be a constant somewhere.
			};
			_conversationService.AddSystemAnswer(conversation, systemAnswer);
		}
		else
		{
			string sparqlQuery = _sparqlTranslationService.TranslateSubstructure(substructure);
			StringBuilder answerBuilder = new StringBuilder();
			answerBuilder.Append("The data you want can be retrieved using the following Sparql query: ");
			answerBuilder.Append(sparqlQuery);
			answerBuilder.AppendLine();
			answerBuilder.AppendLine();
			answerBuilder.Append("Some parts of your question can be expanded and I have highlighted the relevant words. You can click on them for more information.");

			var systemAnswer = new PositiveSystemAnswer()
			{
				Id = systemAnswerId,
				TimeStamp = DateTime.Now,
				TextValue = answerBuilder.ToString()
			};
			systemAnswer.MatchedItems = _dataSpecificationService.GetHighlightableItems(conversation);
			// To do: I will go the route without highlighting to make it easier.
		}

		return Results.Created(
			uri: $"/conversations/{conversationId}/messages/{userMessage.Id}",
			value: new MessageDetailedDTO()
			{
				Type = MessageType.UserMessage,
				Id = userMessage.Id,
				TimeStamp = userMessage.TimeStamp,
				TextValue = userMessage.TextValue,
				SystemAnswerIri = $"/conversations/{conversationId}/messages/{systemAnswerId}"
			}
		);
	}
	#endregion
}
