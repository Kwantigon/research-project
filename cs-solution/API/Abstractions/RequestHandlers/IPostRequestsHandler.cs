using Backend.DTO;

namespace Backend.Abstractions.RequestHandlers;

public interface IPostRequestsHandler
{
	/// <summary>
	/// Receive and process information for a new data specification.
	/// </summary>
	/// <param name="payload">The request's body.</param>
	/// <returns>???</returns>
	IResult PostDataSpecifications(PostDataSpecificationsRequestDTO payload);

	IResult PostConversations(PostConversationsRequestDTO payload);

	void PostConversationMessages();
}
