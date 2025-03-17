using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Model;
using System.Net;

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
			throw new Exception("Could not find the requested data specification");
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
			throw new Exception("Could not find the requested conversation");
		}

		var message = conversation.AddUserMessage(payload.TextValue, payload.TimeStamp);
		return Results.Created(
			uri: $"/conversations/{conversationId}/messages/{message.Id}",
			value: new MessageBasicDTO()
			{
				Source = MessageSource.User,
				TimeStamp = message.TimeStamp,
				Text = message.TextValue
			});
	}
	#endregion
}
