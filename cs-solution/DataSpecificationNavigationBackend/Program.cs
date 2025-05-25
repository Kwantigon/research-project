using DataspecNavigationHelper.BusinessCoreLayer;
using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddSingleton<IConversationService, ConversationServiceMock>()
	.AddSingleton<IConversationController, ConversationController>()
	.AddSingleton<IDataSpecificationService, DataSpecificationService>()
	.AddSingleton<IDataSpecificationController, DataSpecificationController>()
	.AddSingleton<IDataspecerConnector, DataspecerConnectorMock>()
	.AddSingleton<EntityFrameworkPlaceholder>()
	;

var app = builder.Build();

/* I'm registering all endpoints in this Program.cs file
 * because I don't expect to have many more endpoints than the ones listed here.
 * 
 * An alternative would be to create extension methods for grouping endpoints
 * and registering them.
 */

// Sanity check.
app.MapGet("/", () => "Hello there!");

app.MapGet("/conversations",
			(IConversationController controller) => controller.GetOngoingConversations())
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get all ongoing conversations.";
		endpoint.Description = "Front end calls this to display all conversations in the conversations management tab. The front end will show the title of each conversation and when the user expands the conversation, they will see more information about it.";
		return endpoint;
	});

app.MapGet("/conversations/{conversationId}",
			([FromRoute] int conversationId, IConversationController controller) => controller.GetConversation(conversationId))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get information about the conversation.";
		endpoint.Description = "This endpoint is only for debugging. The front end does not need to call this for anything.";
		return endpoint;
	});

app.MapGet("/conversations/{conversationId}/messages",
			([FromRoute] int conversationId, IConversationController controller) => controller.GetConversationMessages(conversationId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get all messages in the conversation.";
		operation.Description = "Returns all messages ordered by their timestamps. The front end calls this when it loads a conversation and needs to display messages in the conversation.";
		return operation;
	});

app.MapGet("/conversations/{conversationId}/messages/{messageId}",
			([FromRoute] int conversationId, [FromRoute] int messageId,
			IConversationController controller) => controller.GetMessage(conversationId, messageId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get the concrete message from a conversation.";
		operation.Description = "Returns all available information about the requested message. The front end calls this to get the reply to an user's message.";
		return operation;
	});

app.MapPost("/conversations/{conversationId}/messages",
				([FromRoute] int conversationId,
				[FromBody] PostConversationMessagesDTO payload,
				IConversationController controller) => controller.ProcessUserMessage(conversationId, payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Add a message to the conversation.";
		operation.Description = "The message that should be added is always assumed to be an user message. Returns the created message that also contains the IRI of the reply message. The front end calls this endpoint to add the user's message to the conversation. It will then call the reply message's IRI to get the system's answer. This operation is currently synchronous. I might change it to an asynchronous operation later down the line.";
		return operation;
	});

app.MapPost("/data-specifications",
				([FromBody] PostDataSpecificationsDTO payload,
				IDataSpecificationController controller) => controller.ProcessDataspecerPackage(payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Add a new data specification.";
		operation.Description = "Exports and processes the necessary data from the Dataspecer package given in the payload's IRI. If a name is given, the processed data specification will be stored under that name, otherwise a default name will be used.";
		return operation;
	});


app.Run();
