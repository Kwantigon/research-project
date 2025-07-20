using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.BusinessCoreLayer.Abstraction;
using DataspecNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataspecNavigationBackend.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IConversationService""/> to HTTP responses.
/// </summary>
public class ConversationController(
	IConversationService conversationService,
	IDataSpecificationService dataSpecificationService,
	AppDbContext appDbContext) : IConversationController
{
	private readonly IConversationService _conversationService = conversationService;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	private readonly AppDbContext _database = appDbContext; // Only for mocks.

	public async Task<IResult> StartConversation(PostConversationsDTO payload)
	{
		DataSpecification? dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecerAsync(payload.DataspecerPackageUuid, payload.DataspecerPackageName);
		if (dataSpecification is null)
		{
			return Results.InternalServerError(new Error() { Reason = "There was an error while retrieving and processing the Dataspecer package." });
		}
		Conversation conversation = await _conversationService.StartNewConversationAsync(payload.ConversationTitle, dataSpecification);
		return Results.Created($"/conversations/{conversation.Id}", (ConversationDTO)conversation);
	}

	public async Task<IResult> GetOngoingConversations()
	{
		IReadOnlyList<Conversation> conversations = await _conversationService.GetAllConversationsAsync();
		return Results.Ok(
			conversations.Select(conv => (ConversationDTO)conv)
		);
	}

	public async Task<IResult> GetConversation(int conversationId)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation is null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}
		
		return Results.Ok((ConversationDTO)conversation);
	}

	public async Task<IResult> GetConversationMessagesAsync(int conversationId)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId, includeMessages: true);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		return Results.Ok(
			conversation.Messages.Select(msg => (ConversationMessageDTO)msg)
		);
	}

	public async Task<IResult> GetMessageAsync(int conversationId, Guid messageId)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId, includeMessages: true);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		Message? message = conversation.Messages.Find(msg => msg.Id == messageId);
		if (message == null)
		{
			return Results.NotFound(new Error { Reason = $"Message with ID {messageId} not found." });
		}

		// Verify that the messageId matches the last element in the messages list.
		// That should be the reply message that has to be processed.
		if (conversation.Messages.Last() == message && message.TextValue == string.Empty)
		{
			// In this case, the reply message has not yet been generated.
			// Todo: Generate the reply.
		}

		return Results.Ok((ConversationMessageDTO)message);
	}

	public async Task<IResult> ProcessUserMessageAsync(int conversationId, PostConversationMessagesDTO payload)
	{
		Conversation? conversation = await _conversationService.GetConversationAsync(conversationId);
		if (conversation == null)
		{
			return Results.NotFound(new Error { Reason = $"Conversation with ID {conversationId} not found." });
		}

		//Message userMessage = _conversationService.AddUserMessage(conversationId, payload.TimeStamp, payload.TextValue);

		// Map the user's question to items from the data specification.

		// Get a list of data specification items that are relevant to the question.

		// Translate to Sparql.

		// Create a reply message and add that message to the conversation. The reply message also contains the list of relevant items.

		//return Results.Created($"/conversations/{conversationId}/messages/{userMessage.Id}", userMessage);
		return Results.Ok();
		// Todo: Return "replyUri" as part of the answer so that the front end knows, what URI to query for the reply.
	}

	/*public IResult StartEfTestConversation(PostConversationsDTO payload)
	{
		DataSpecification ds = _database.DataSpecifications.Single(spec => spec.DataspecerPackageUuid.Equals(payload.DataspecerPackageUuid));
		if (ds is null)
		{
			ds = new DataSpecification(0, $"some-iri-{DateTime.Now.ToString()}", "mock", "owl placeholder");
			_database.DataSpecifications.Add(ds);
		}

		Conversation conversation = new(0, payload.ConversationTitle, ds.Id, ds, [], DateTime.Now);
		_database.Conversations.Add(conversation);
		_database.SaveChanges();
		return Results.Created($"/ef-test/conversations/{conversation.Id}", conversation);
	}*/

	public async Task<IResult> DeleteConversationAsync(int conversationId)
	{
		Conversation? conversation = await _database.Conversations.SingleOrDefaultAsync(c => c.Id == conversationId);
		if (conversation is null)
		{
			return Results.NotFound($"Conversation with ID {conversationId} not found.");
		}

		_database.Conversations.Remove(conversation);
		_database.SaveChanges();
		return Results.NoContent();
	}
}
