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
				ErrorMessage = "No URI to a Dataspecer package was given"
			});
		}

		DataSpecification dataSpecNew = new DataSpecification(payload.Name, payload.IriToDataspecer);
		return Results.Created(uri: "/data-specifications/0", string.Empty);
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

		DataSpecification dataSpecification = _database.GetDataSpecificationById(dataSpecificationId);
		Conversation conversation = new Conversation(dataSpecification, payload.ConversationTitle);
		_database.AddNewConversation(conversation);
		return Results.Created($"/conversations/{conversation.Id}", string.Empty);
	}

	public void PostConversationMessages()
	{
		throw new NotImplementedException();
	}
	#endregion
}
