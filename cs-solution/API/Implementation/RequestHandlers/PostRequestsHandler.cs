using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;
using System.Text;

namespace Backend.Implementation.RequestHandlers;

public class PostRequestsHandler(
	ILogger<PostRequestsHandler> logger,
	IDatabase database,
	IDataSpecificationService dataSpecificationService,
	IConversationService conversationService,
	ISparqlTranslationService sparqlTranslationService,
	IPromptConstructor promptConstructor,
	ILlmConnector llmConnector,
	ILlmResponseProcessor llmResponseProcessor) : IPostRequestsHandler
{
	private readonly ILogger<PostRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	private readonly IConversationService _conversationService = conversationService;
	private readonly ISparqlTranslationService _sparqlTranslationService = sparqlTranslationService;
	private readonly IPromptConstructor _promptConstructor = promptConstructor;
	private readonly ILlmConnector _llmConnector = llmConnector;
	private readonly ILlmResponseProcessor _llmResponseProcessor = llmResponseProcessor;

	#region Interface implementation.
	public IResult PostDataSpecifications(PostDataSpecificationsRequestDTO payload)
	{
		if (string.IsNullOrEmpty(payload.IriToDataspecer))
		{
			return Results.BadRequest(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "No IRI to a Dataspecer package was given"
			});
		}

		DataSpecification dataSpecification = _dataSpecificationService.CreateDataSpecificationFromDataspecerPackage(payload.IriToDataspecer);
		dataSpecification.Name = payload.Name ?? $"Unnamed data specification #{dataSpecification.Id}";

		if (_database.AddNewDataSpecification(dataSpecification) is false)
		{
			_logger.LogError("Failed to add the data specification {DataSpec} to the database.", dataSpecification);
			return Results.InternalServerError(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.InternalServerError,
				ErrorMessage = "Failed to create a new data specification"
			});
		}
		else
		{
			return Results.Created(uri: $"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
		}
	}

	public IResult PostConversations(PostConversationsRequestDTO payload)
	{
		if (string.IsNullOrEmpty(payload.DataSpecificationIri))
		{
			return Results.BadRequest(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The data specification's IRI was either null or empty"
			});
		}

		string[] iriParts = payload.DataSpecificationIri.Split('/');
		// The iri looks like this: /data-specifications/{dataSpecificationId}
		// So iriParts will be: [ "", "data-specification", "{dataSpecificationId}" ]
		if (iriParts.Length != 3)
		{
			_logger.LogError("The iriParts has an unexpected length of {IriPartsLength}. IRI: {0}", iriParts.Length, payload.DataSpecificationIri);
			return Results.BadRequest(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The data specification's IRI has an unexpected format"
			});
		}
		if (uint.TryParse(iriParts[2], out uint dataSpecificationId) is false)
		{
			_logger.LogError("Failed to parse {ID} as an uint.", iriParts[2]);
			return Results.BadRequest(new ErrorResponseDTO
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

		Conversation conversation = _conversationService.CreateNewConversation(dataSpecification);
		if (_database.AddNewConversation(conversation) is false)
		{
			_logger.LogError("Failed to add the conversation {Conversation} to the database.", conversation);
			return Results.InternalServerError(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.InternalServerError,
				ErrorMessage = "Failed to start a new conversation"
			});
		}

		return Results.Created($"/conversations/{conversation.Id}", (ConversationDTO)conversation);
	}

	// To do: This method should be async.
	// But when converting to async, I have to figure out how to handle the ID of the SystemAnswer.
	public IResult PostConversationMessages(uint conversationId, PostConversationMessagesDTO payload)
	{
		if (string.IsNullOrEmpty(payload.TextValue))
		{
			return Results.BadRequest(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.BadRequest,
				ErrorMessage = "The text in the message was either null or empty"
			});
		}

		var conversation = _database.GetConversationById(conversationId);
		if (conversation is null)
		{
			_logger.LogError("Failed to retrieve the conversation with ID {ConversationId} from the database.", conversationId);
			return Results.NotFound(new ErrorResponseDTO
			{
				ErrorCode = HttpStatusCode.NotFound,
				ErrorMessage = $"Failed to find the conversation with ID {conversationId}"
			});
		}

		UserMessage userMessage = _conversationService.AddAndReturnUserMessageToConversation(conversation, payload.TextValue, payload.TimeStamp);

		// Jsou tu 3 moznosti:
		// Jedna se o uplne prvni zpravu od uzivatele
		// Jedna se o preview zpravu, kterou uzivatel jen potvrdil.
		// Jedna se o preview zpravu, kterou uzivatel upravil.
		DataSpecificationSubstructure? substructure = null;
		List<DataSpecificationItem> mappedItems = [];
		if (conversation.State is ConversationState.AwaitingFirstUserMessage)
		{
			string itemsMappingPrompt = _promptConstructor.CreateItemsMappingPrompt(payload.TextValue);
			string itemsMappingResponse = _llmConnector.SendPromptAndReceiveResponse(itemsMappingPrompt);
			mappedItems = _llmResponseProcessor.ProcessItemsMappingResponse(itemsMappingResponse);
			// To do: If the response processor fails to parse, it's likely because the LLM returned an invalid answer format.
			// Try to resend the prompt and ask for the correct answer format.

			substructure = _conversationService.CreateDataSpecificationSubstructureForConversation(conversation, mappedItems);
		}
		
		if (conversation.State is ConversationState.AwaitingFollowUpUserMessage)
		{
			if (payload.UserModifiedPreviewMessage)
			{
				// I don't know how much the user has modified.
				// If they completely changed the question, then it has to be processed as if it was the first question.
				// But maybe they only changed a little bit and I can somehow just change the preview substructure a bit?

				// I think for now, just process like the first message.
				string itemsMappingPrompt = _promptConstructor.CreateItemsMappingPrompt(userMessage.TextValue);
				string itemsMappingResponse = _llmConnector.SendPromptAndReceiveResponse(itemsMappingPrompt);
				mappedItems = _llmResponseProcessor.ProcessItemsMappingResponse(itemsMappingResponse);
				substructure = _conversationService.CreateDataSpecificationSubstructureForConversation(conversation, mappedItems);
			}
			else
			{
				substructure = _conversationService.ConfirmSubstructurePreview(conversation);
			}
		}

		SystemAnswer systemAnswer;
		if (substructure is null)
		{
			systemAnswer = new NegativeSystemAnswer { Id = conversation.NextUnusedMessageId++, TextValue = "Mock negative answer.", TimeStamp = DateTime.Now };
		}
		else
		{
			string sparqlQuery = _sparqlTranslationService.TranslateSubstructure(substructure);
			systemAnswer = _conversationService.GeneratePositiveSystemAnswer(sparqlQuery, mappedItems, conversation);
		}
		
		// To do: This should really be elsewhere.
		conversation.Messages.Add(systemAnswer);
		conversation.State = ConversationState.AwaitingFollowUpUserMessage;
		userMessage.SystemAnswer = systemAnswer;

		return Results.Created(
			uri: $"/conversations/{conversationId}/messages/{userMessage.Id}",
			value: new MessageDetailedDTO()
			{
				Type = MessageType.UserMessage,
				Id = userMessage.Id,
				TimeStamp = userMessage.TimeStamp,
				TextValue = userMessage.TextValue,
				SystemAnswerIri = $"/conversations/{conversationId}/messages/{systemAnswer.Id}"
			}
		);
	}
	#endregion
}
