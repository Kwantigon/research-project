using BackendApi.DTO;
using Microsoft.AspNetCore.Mvc;
using RequestHandler;

// ToDo: Create DTOs for all request mappings.

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

builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

#region Handlers inicialization
//GetRequestsHandler getRequestsHandler = new GetRequestsHandler();
#endregion

#region Requests
app.MapGet("/", () => "Hello there!")
	.WithOpenApi(o =>
	{
		o.Summary = "Sanity check.";
		o.Description = "A simple sanity check. Should return the \"Hello there!\" string.";
		return o;
	});

app.MapPost(
	"/data-specifications",
	([FromBody] PostDataSpecificationsRequestDTO dataSpecificationInfo) =>
	{
		uint dataSpecificationId = Handler.POST.ProcessNewDataSpecification(dataSpecificationInfo);
		string createdUri = "/data-specifications/" + dataSpecificationId;
		return Results.Created(uri: createdUri, string.Empty);
	})
	.WithOpenApi(o =>
	{
		o.Summary = "Add a new data specification.";
		o.Description = "Expects an URI, which points to a Dataspecer package. The server will process this package and store the internal representation of the package, further referred to as a data specification. Once stored, new conversations about this data specification can be created and users can query about this data specification. If a name is given, the server will store the data specification under that name, otherwise a default name will be used.";
		return o;
	});

app.MapGet("/data-specifications", () => Handler.GET.AllDataSpecifications())
	.WithOpenApi(o =>
	{
		o.Summary = "Get all data specifications stored on the server.";
		o.Description = "Returns information about each data specification stored on the server.";
		return o;
	});

app.MapGet("/data-specifications/{dataSpecificationId}", ([FromRoute] uint dataSpecificationId) => Handler.GET.DataSpecification(dataSpecificationId))
	.WithOpenApi(o =>
	{
		o.Summary = "Get details of a data specification";
		o.Description = "Returns all available information about the data specification with the given ID.";
		return o;
	});

app.MapPost(
	"/conversations",
	([FromBody] PostConversationsRequestDTO postConversationsRequestDTO) =>
	{
		uint conversationId = Handler.POST.CreateConversation(postConversationsRequestDTO);
		string createdUri = "/conversations/" + conversationId;
		return Results.Created(uri: createdUri, string.Empty);
	})
	.WithOpenApi(o =>
	{
		o.Summary = "Start a new conversation.";
		o.Description = "Create a new conversation using the data specification given in the request body. Once created, messages can be sent into the conversation. If a title is given, the server will store the conversation under that title, otherwise a default title will be used.";
		return o;
	});

app.MapGet("/conversations", () => Handler.GET.AllConversations())
	.WithOpenApi(o =>
	{
		o.Summary = "Get all ongoing conversations.";
		o.Description = "Returns the location and name of all conversations stored on the server.";
		return o;
	});

app.MapGet("/conversations/{conversationId}/messages", ([FromRoute] uint conversationId) => Handler.GET.ConversationMessages(conversationId))
	.WithOpenApi(o =>
	{
		o.Summary = "Get all messages in the conversation.";
		o.Description = "Returns an array of messages in the conversation.";
		return o;
	});

// Problem: How do the API users know that there is this resource available?
// I did not return the location for the property-summary calls.
// Maybe make the location available in the system's response in the user's message?
app.MapGet("/data-specifications/{dataSpecificationId}/property-summary", ([FromRoute] uint dataSpecificationId, [FromQuery] uint propertyId) => $"Property summary for property with ID={propertyId} of data specification with ID={dataSpecificationId}.")
	.WithOpenApi(o =>
	{
		o.Summary = "Get a summary for a property.";
		o.Description = "Returns a summary of the requested property in the data specification with the given ID.";
		return o;
	});


app.MapGet("/conversations/{conversationId}/messages/{messageId}/system-response", () => "GET .../system-response");

app.MapGet("/conversations/{conversationId}/messages/{messageId}/question-preview", () => "GET .../question-preview");

// Might break idempotency because the LLM could generate a different lexicalization for every call with the same parameters.
app.MapPut("/conversations/{conversationId}/next-message-preview", (/*All properties that user has selected for the next message*/) => { });

/*app.MapPost(
	"/conversations/{conversationId}/messages",
	([FromRoute] uint conversationId, [FromBody] PostConversationMessageDTO messageDTO) => Handler.POST.AddNewMessageToConversation(conversationId, messageDTO)
).WithOpenApi(o =>
{
	o.Summary = "Add a message to the conversation.";
	o.Description = "Add a message to the conversation with the given conversationId. If the message's source is the user, further process the message (to extract relevant entities etc.). Returns the ID of the newly added message.";
	return o;
});

app.MapGet(
	"/conversations/{conversationId}/messages/{messageId}/sparql-query",
	([FromRoute] uint conversationId, [FromRoute] uint messageId) => { Console.WriteLine("GET /conversations/{conversationId}/user-messages/{messageId}/sparql-query"); }
).WithOpenApi(o =>
{
	o.Summary = "Get the Sparql query corresponding to the user's message.";
	o.Description = "Each user's message is processed to extract relevant properties and the message is also translated into a Sparql query. This resource returns the translated Sparql query. The messageId must be an user's message, otherwise this resource will return an error.";
	return o;
});

app.MapGet(
	"/conversations/{conversationId}/messages/{messageId}/highlighted-words",
	([FromRoute] uint conversationId, [FromRoute] uint messageId) => { Console.WriteLine("GET /conversations/{conversationId}/user-messages/{messageId}/highlighted-words"); }
).WithOpenApi(o =>
{
	o.Summary = "Get words that should be highlighted in the user's message.";
	o.Description = "Highlighted words are mapped to properties in the data specification. This resource returns the positions of the words to be highlighted and the IDs of the properties that they map to. The messageId must be an user's message, otherwise this resource will return an error.";
	return o;
});*/

#endregion

app.Run();
