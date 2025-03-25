using Backend.Abstractions;
using Backend.Abstractions.RequestHandlers;
using Backend.DTO;
using Backend.Implementation;
using Backend.Implementation.Database;
using Backend.Implementation.RequestHandlers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.SetIsOriginAllowed(origin => true);
	});
});

#region Services for dependency injection

builder.Services
	.AddSwaggerGen()
	.AddSingleton<IDatabase, InMemoryDatabase>()
	.AddSingleton<IConversationService, ConversationService>()
	.AddSingleton<IDataSpecificationService, DataSpecificationService>()
	.AddSingleton<ISparqlTranslationService, SparqlTranslationService>()
	.AddSingleton<IPromptConstructor, MockPromptConstructor>()
	.AddSingleton<ILlmConnector, MockLlmConnector>()
	.AddSingleton<ILlmResponseProcessor, MockLlmResponseProcessor>()
	.AddSingleton<IGetRequestsHandler, GetRequestsHandler>()
	.AddSingleton<IPostRequestsHandler, PostRequestsHandler>()
	.AddSingleton<IPutRequestsHandler, PutRequestsHandler>()
	;
#endregion

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

#region GET requests.

app.MapGet("/", () => "Hello there!")
	.WithOpenApi(operation =>
	{
		operation.Summary = "Sanity check.";
		operation.Description = "A simple sanity check. Should return the \"Hello there!\" string.";
		return operation;
	});

app.MapGet("/about", (IGetRequestsHandler handler) => handler.GetAbout());

app.MapGet("/data-specifications", (IGetRequestsHandler handler) => handler.GetAllDataSpecifications())
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get all data specifications stored on the server.";
		operation.Description = "Returns information about each data specification stored on the server.";
		return operation;
	});

app.MapGet("/data-specifications/{dataSpecificationId}",
				([FromRoute] uint dataSpecificationId,
				IGetRequestsHandler handler) => handler.GetDataSpecification(dataSpecificationId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get details of a data specification";
		operation.Description = "Returns all available information about the data specification with the given ID.";
		return operation;
	});

app.MapGet("/conversations", (IGetRequestsHandler handler) => handler.GetAllConversations())
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get all ongoing conversations.";
		operation.Description = "Returns the location and name of all conversations stored on the server.";
		return operation;
	});

app.MapGet("/conversations/{conversationId}", ([FromRoute] uint conversationId, IGetRequestsHandler handler) => handler.GetConversation(conversationId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get basic information about the conversation.";
		operation.Description = "Returns basic information about the conversation with the requested ID.";
		return operation;
	});

app.MapGet("/conversations/{conversationId}/messages", ([FromRoute] uint conversationId, IGetRequestsHandler handler) => handler.GetConversationMessages(conversationId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get all messages in the conversation.";
		operation.Description = "Returns all messages in the conversation. Each message will contain only the most basic information, which is its source (either user or system), timestamp and the textual value. Messages are ordered by their timestamp.";
		return operation;
	});

app.MapGet("/conversations/{conversationId}/messages/{messageId}",
			([FromRoute] uint conversationId,
			[FromRoute] uint messageId,
			IGetRequestsHandler handler) => handler.GetMessageFromConversation(conversationId, messageId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get details of a concrete message.";
		operation.Description = "Returns all available information about the requested message. Typically the front end will want to request a previously POSTed user's message to retrieve the system's answer to the message.";
		return operation;
	});

app.MapGet("/data-specifications/{dataSpecificationId}/items/{itemId}/summary",
			([FromRoute] uint dataSpecificationId,
			[FromRoute] uint itemId,
			IGetRequestsHandler handler) => handler.GetItemSummaryFromDataSpecification(dataSpecificationId, itemId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get a summary for a property.";
		operation.Description = "Returns a summary of the requested property and IRIs to get summaries of other properties that are related to the requested property..";
		return operation;
	});

app.MapGet("/conversations/{conversationId}/next-message-preview",
			([FromRoute] uint conversationId,
			IGetRequestsHandler handler) => handler.GetNextMessagePreview(conversationId))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Get the natural language equivalent of the expanded query.";
		operation.Description = "Returns a preview of the user's previous query expanded by their selected expansion properties. This preview will be in natural language and is meant to be displayed to the user.";
		return operation;
	});
#endregion

#region POST requests.

app.MapPost("/data-specifications",
				([FromBody] PostDataSpecificationsRequestDTO payload,
				IPostRequestsHandler handler) => handler.PostDataSpecifications(payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Add a new data specification.";
		operation.Description = "Expects an IRI, which points to a Dataspecer package. The server will process this package and store the internal representation of the package, further referred to as a data specification. Once stored, new conversations about this data specification can be created and users can query about this data specification. If a name is given, the server will store the data specification under that name, otherwise a default name will be used.";
		return operation;
	});

app.MapPost("/conversations",
				([FromBody] PostConversationsRequestDTO payload,
				IPostRequestsHandler handler) => handler.PostConversations(payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Start a new conversation.";
		operation.Description = "Create a new conversation using the data specification given in the request body. Once created, messages can be sent into the conversation. If a title is given, the server will store the conversation under that title, otherwise a default title will be used.";
		return operation;
	});

app.MapPost("/conversations/{conversationId}/messages",
				([FromRoute] uint conversationId,
				[FromBody] PostConversationMessagesDTO payload,
				IPostRequestsHandler handler) => handler.PostConversationMessages(conversationId, payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Add a message to the conversation.";
		operation.Description = "Adds a new message to the existing conversation and returns an IRI to that message's location. For now, this is a synchronous operation. I might change it to be asynchronous if necessary.";
		return operation;
	});

#endregion

#region PUT requests.

app.MapPut("/conversations/{conversationId}/next-message-preview",
				([FromRoute] uint conversationId,
				[FromBody] PutConversationNextMessagePreviewDTO payload,
				IPutRequestsHandler handler) => handler.PutConversationNextMessagePreview(conversationId, payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Send the properties that user has selected for preview.";
		operation.Description = "Use the user's selected properties to generate a question in natural language. The selected properties and the message preview WILL NOT be added to the conversation yet. (On the back end side: create a temporary substructure of the data specification, which corresponds to the query expanded by these selected properties).";
		return operation;
	});

#endregion

app.Run();
