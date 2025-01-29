using BackendApi.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RequestHandler;

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

//builder.Services.AddOpenApi("api");
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();
//app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

#region Requests

///
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
		return Results.Created(uri: createdUri, $"{{ \"dataSpecificationId\": {dataSpecificationId} }}");
	}
).WithOpenApi(o =>
{
	o.Summary = "Add a new data specification.";
	o.Description = "Process the given URI, which should point to a data specification. Add that data specification to the database. If name is not specified, it will be named \"Unnamed data specification #{ID}\".";
	return o;
});

app.MapGet("/data-specifications", () => Handler.GET.AllDataSpecifications())
	.WithOpenApi(o =>
	{
		o.Summary = "Get ID and Name of all data specifications.";
		o.Description = "Returns the ID and Name of each data specification in the database. (ToDo: In the future maybe add a limit to the query part)";
		return o;
	});

app.MapGet("/data-specifications/{dataSpecificationId}", ([FromRoute] uint dataSpecificationId) => Handler.GET.DataSpecification(dataSpecificationId))
	.WithOpenApi(o =>
	{
		o.Summary = "Get details of a data specification";
		return o;
	});

app.MapPost(
	"/conversations",
	([FromBody] PostConversationsRequestDTO postConversationsRequestDTO) =>
	{
		uint conversationId = Handler.POST.CreateConversation(postConversationsRequestDTO);
		string createdUri = "/conversations/" + conversationId;
		return Results.Created(uri: createdUri, $"{{ \"conversationId\": {conversationId} }}");
	}
).WithOpenApi(o =>
{
	o.Summary = "Start a new conversation.";
	o.Description = "Create a new conversation over the data specification with the given ID. The data specification must be already present in the database. If name is not specified, it will be named \"Unnamed conversation #{ID}\".";
	return o;
});

app.MapGet("/conversations", () => Handler.GET.AllConversations())
	.WithOpenApi(o =>
	{
		o.Summary = "Get ID and Name of all conversations.";
		o.Description = "Returns the ID and Name of each conversation in the database. (ToDo: In the future maybe add a limit to the query part)";
		return o;
	});

app.MapGet("/conversations/{conversationId}", ([FromRoute] uint conversationId) => Handler.GET.ConversationMessages(conversationId))
	.WithOpenApi(o =>
	{
		o.Summary = "Get all messages in the conversation.";
		o.Description = "Returns an array of messages in the conversation. Each message contains the message's source (user or chatbot), a timestamp of when the message was added to the conversation and the string value of the message.";
		return o;
	});

/*app.MapPost(
	"/conversations/{conversationId}/bot-message",
	([FromRoute] uint conversationId, [FromBody] PostConversationMessageDTO messageDTO) =>
	{
		Handler.POST.AddBotMessageToConversation(conversationId, messageDTO);
		return Results.Accepted();
	}
);*/

app.MapPost(
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
});

/*app.MapGet("/conversations/{conversationId}/property-summary", ([FromRoute] uint conversationId, [FromQuery] int propertyId) => Handler.GET.PropertySummary(propertyId));*/

app.MapGet("/data-specifications/{dataSpecificationId}/property-summary", ([FromRoute] uint dataSpecificationId, [FromQuery] uint propertyId) => Handler.GET.PropertySummary(dataSpecificationId, propertyId))
	.WithOpenApi(o =>
	{
		o.Summary = "Get a summary of the specified property.";
		o.Description = "Get a summary of the property with the given propertyId from the data specification with the given ID.";
		return o;
	});

#endregion

app.Run();
