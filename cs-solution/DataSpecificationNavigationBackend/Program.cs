using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.BusinessCoreLayer.Facade;
using DataSpecificationNavigationBackend.ConnectorsLayer.LlmConnectors;
using DataspecNavigationHelper.BusinessCoreLayer;
using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.BusinessCoreLayer.Facade;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddSingleton<IConversationService, ConversationService>()
	.AddSingleton<IConversationController, ConversationController>()
	.AddSingleton<IDataSpecificationService, DataSpecificationService>()
	.AddSingleton<IDataSpecificationController, DataSpecificationController>()
	.AddSingleton<IDataspecerConnector, DataspecerConnector>()
	.AddSingleton<ILlmConnector, GeminiConnector>()
	.AddSingleton<IRdfProcessor, RdfProcessor>()
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

/*
 * Po získání Dataspecer package budu potřebovat vrátit nějaký redirect nebo něco.
 * V Dataspecer manager kliknu na tlačítko, které pošle IRI package sem a zároveň mě to musí
 * vzít do front end mé aplikace, kde po načtení Dataspecer package můžu rovnou chatovat.
 * 
 * Dejme tomu, že Štěpán implementuje do Dataspeceru to tlačítko, že pošle POST /data-specifications a přesměruje uživatele.
 * Ale kam má uživatele přesměrovat?
 * Tzn. musím přidat další endpoint, který počká na načtení package a pak začne konverzaci.
 * Nebo to nějak vhodně implementovat do endpointu POST /conversations (ten tu ještě nemám).
 * 
 * Udělám to asi tak, že Štěpán implementuje to tlačítko. To tlačítko pošle IRI package na front end a přesměruje.
 * Front end vezme to package IRI a pošle POST /data-specifications na back end.
 * Až back end vrátí odpověď OK, tak front end pokračuje voláním POST /conversations.
 * 
 * Mezitím co to probíhá front end zobrazí točící kolečko, aby uživatel věděl, že něco načítá.
 */
app.MapPost("/data-specifications",
				([FromBody] PostDataSpecificationsDTO payload,
				IDataSpecificationController controller) => controller.ProcessDataspecerPackage(payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Add a new data specification.";
		operation.Description = "Exports and processes the necessary data from the Dataspecer package given in the payload's IRI. If a name is given, the processed data specification will be stored under that name, otherwise a default name will be used.";
		return operation;
	});

app.MapPost("/conversations",
				([FromBody] PostConversationsDTO payload,
				IConversationController controller) => controller.StartConversation(payload))
	.WithOpenApi(operation =>
	{
		operation.Summary = "Start a new conversation.";
		operation.Description = "Starts a new conversation with the given title and using the given data specification in the payload. If the conversation title is not specified, a default name will be used instead.";
		return operation;
	});

app.Run();
