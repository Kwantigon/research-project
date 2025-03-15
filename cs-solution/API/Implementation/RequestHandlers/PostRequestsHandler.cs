using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;

namespace Backend.Implementation.RequestHandlers;

public class PostRequestsHandler(ILogger<PostRequestsHandler> logger, IDatabase database) : IPostRequestsHandler
{
	private readonly ILogger<PostRequestsHandler> _logger = logger;
	private readonly IDatabase _database = database;

	#region Interface implementation.
	public IResult PostDataSpecifications(PostDataSpecificationsRequestDTO payload)
	{
		_logger.LogDebug("Payload is: {P}", payload);
		return Results.Created(uri: "/data-specifications/0", string.Empty);
	}

	public void PostConversations()
	{
		throw new NotImplementedException();
	}

	public void PostConversationMessages()
	{
		throw new NotImplementedException();
	}
	#endregion
}
