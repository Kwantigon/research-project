using DataSpecificationNavigatorBackend.BusinessCoreLayer;
using DataSpecificationNavigatorBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigatorBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigatorBackend.BusinessCoreLayer.Facade;
using DataSpecificationNavigatorBackend.ConnectorsLayer;
using DataSpecificationNavigatorBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigatorBackend.ConnectorsLayer.LlmConnectors;
using DataSpecificationNavigatorBackend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	// Scoped services (created per request).
	.AddScoped<IConversationService, ConversationService>()
	.AddScoped<IConversationController, ConversationController>()
	.AddScoped<IDataSpecificationService, DataSpecificationService>()
	.AddScoped<IDataSpecificationController, DataSpecificationController>()
	.AddScoped<IDataspecerConnector, DataspecerConnector>()
	.AddScoped<ILlmConnector, GeminiConnector>()
	.AddScoped<IRdfProcessor, RdfProcessor>()
	.AddScoped<ILlmResponseProcessor, ResponseProcessor>()
	.AddScoped<ILlmConnector, GeminiConnector>()
	.AddScoped<ISparqlTranslationService, SparqlTranslationService>()

	.AddSingleton<IPromptConstructor, PromptConstructor>() // Singleton because I don't want to load templates from files every time there is a request.
	;

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
/*builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo {
		Title = "Data specification helper API",
		Description = "The back end for the data specification helper project.",
		Version = "v1" }
	);
});*/

var connectionString = builder.Configuration.GetConnectionString("Chatbot") ?? "Data Source=Chatbot.db";
builder.Services.AddDbContext<AppDbContext>(b => b.UseSqlite(connectionString).UseLazyLoadingProxies());

var app = builder.Build();
app.UseCors();
/*if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data specification helper API");
	});
}*/

// Sanity check.
app.MapGet("/", () => "Hello there!");

app.MapGet("/conversations",
			async (IConversationController controller) => await controller.GetAllConversationsAsync())
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get all ongoing conversations.";
		endpoint.Description = "Front end calls this to display all conversations in the conversations management tab.";
		return endpoint;
	});

app.MapGet("/conversations/{conversationId}",
			async ([FromRoute] int conversationId, IConversationController controller) => await controller.GetConversationAsync(conversationId))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get information about the conversation.";
		endpoint.Description = "This endpoint is only for debugging. The front end does not need to call this for anything.";
		return endpoint;
	});

app.MapGet("/conversations/{conversationId}/messages",
			async ([FromRoute] int conversationId, IConversationController controller) => await controller.GetConversationMessagesAsync(conversationId))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get all messages in the conversation.";
		endpoint.Description = "Returns all messages ordered by their timestamps. The front end calls this when it loads a conversation and needs to display messages in the conversation.";
		return endpoint;
	});

app.MapGet("/conversations/{conversationId}/messages/{messageId}",
			async ([FromRoute] int conversationId, [FromRoute] Guid messageId,
						IConversationController controller) => await controller.GetMessageAsync(conversationId, messageId))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Get the concrete message from a conversation.";
		endpoint.Description = "Returns all available information about the requested message. The front end calls this to get the reply to an user's message.";
		return endpoint;
	});

app.MapGet("/data-specifications/{dataSpecificationId}/items/summary",
			async ([FromRoute] int dataSpecificationId, [FromQuery] string itemIri,
						IDataSpecificationController controller) => await controller.GetItemSummaryAsync(dataSpecificationId, itemIri))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "";
		endpoint.Description = "";
		return endpoint;
	});

app.MapPost("/conversations/{conversationId}/messages",
				async ([FromRoute] int conversationId,
							[FromBody] PostConversationMessagesDTO payload,
							IConversationController controller) => await controller.ProcessUserMessageAsync(conversationId, payload))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Add a message to the conversation.";
		endpoint.Description = "The message that should be added is always assumed to be an user message. Returns the created message that also contains the IRI of the reply message. The front end calls this endpoint to add the user's message to the conversation. It will then call the reply message's IRI to get the system's answer. This endpoint is currently synchronous. I might change it to an asynchronous endpoint later down the line.";
		return endpoint;
	});

app.MapPost("/data-specifications",
				async ([FromBody] PostDataSpecificationsDTO payload,
							IDataSpecificationController controller) => await controller.ProcessDataspecerPackage(payload))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Add a new data specification.";
		endpoint.Description = "Exports and processes the necessary data from the Dataspecer package given in the payload's IRI. If a name is given, the processed data specification will be stored under that name, otherwise a default name will be used.";
		return endpoint;
	});

app.MapPost("/conversations",
				async ([FromBody] PostConversationsDTO payload,
							IConversationController controller) => await controller.StartConversationAsync(payload))
	.WithOpenApi(endpoint =>
	{
		endpoint.Summary = "Start a new conversation.";
		endpoint.Description = "Starts a new conversation with the given title and using the given data specification in the payload. If the conversation title is not specified, a default name will be used instead.";
		return endpoint;
	});

app.MapPut("/conversations/{conversationId}/user-selected-items",
	async ([FromRoute] int conversationId, [FromBody] PutDataSpecItemsDTO payload,
				IConversationController controller) => await controller.AddSelectedItemsAndGetSuggestedMessage(conversationId, payload));

app.MapDelete("/conversations/{conversationId}",
				async ([FromRoute] int conversationId,
							IConversationController controller) => await controller.DeleteConversationAsync(conversationId));


// Test endpoints
app.MapGet("/tests/new-conversation", async (AppDbContext database) =>
{
	DataSpecification ds = new()
	{
		DataspecerPackageUuid = "test-package",
		Name = "Test package",
		OwlContent = File.ReadAllText("C:\\Users\\nquoc\\MatFyz\\research-project\\repo\\research-project\\backend\\DataSpecificationNavigatorBackend\\local\\data-specification.owl.ttl")
	};

	Conversation c = new()
	{
		DataSpecification = ds,
		Title = "Conversation title",
		DataSpecificationSubstructure = new(),
		LastUpdated = DateTime.Now
	};

	await database.Conversations.AddAsync(c);
	await database.SaveChangesAsync();

	return Results.Created("", $"{{ DataSpecificationId = {ds.Id}, ConversationId = {c.Id} }}");
});

app.MapGet("/tests/llm/mapping-prompt", async (ILlmConnector llmConnector) =>
{
	DataSpecification ds = new()
	{
		DataspecerPackageUuid = "test-package",
		Name = "Test package",
		OwlContent = File.ReadAllText("C:\\Users\\nquoc\\MatFyz\\research-project\\repo\\research-project\\backend\\DataSpecificationNavigatorBackend\\local\\data-specification.owl.ttl")
	};

	Conversation c = new()
	{
		DataSpecification = ds,
		Title = "Conversation title",
		DataSpecificationSubstructure = new(),
		LastUpdated = DateTime.Now
	};

	UserMessage userMessage = new()
	{
		Id = Guid.NewGuid(),
		Conversation = c,
		Sender = Message.Source.User,
		TextContent = "I want to see public services providing electronic signatures.",
	};

	ReplyMessage replyMessage = new()
	{
		Id = Guid.NewGuid(),
		Conversation = c,
		Sender = Message.Source.System,
		PrecedingUserMessage = userMessage
	};

	userMessage.ReplyMessage = replyMessage;
	userMessage.ReplyMessageId = replyMessage.Id;

	List<DataSpecificationItemMapping> mappings = await llmConnector.MapUserMessageToDataSpecificationAsync(ds, userMessage);
	return Results.Ok();
});

app.MapGet("/tests/add-user-msg", async (AppDbContext database, IConversationService service) =>
{
	Conversation? conversation = await database.Conversations.SingleOrDefaultAsync(c => c.Id == 1);
	if (conversation is null)
	{
		return Results.NotFound();
	}

	UserMessage userMessage = await service.AddNewUserMessageAsync(conversation, "I want public services that provide electronic signatures.", DateTime.Now);

	await database.SaveChangesAsync();
	return Results.Ok();
});

app.MapGet("/tests/generate-reply", async (AppDbContext database, IConversationService service) =>
{
	Conversation? conversation = await database.Conversations.SingleOrDefaultAsync(c => c.Id == 1);
	if (conversation is null)
	{
		return Results.NotFound();
	}

	UserMessage userMessage = await service.AddNewUserMessageAsync(conversation, "I want public services that provide electronic signatures.", DateTime.Now);
	ReplyMessage? replyMessage = await service.GenerateReplyMessageAsync(userMessage);

	await database.SaveChangesAsync();
	return Results.Ok();
});

app.MapGet("/tests/add-then-find", async (AppDbContext database) =>
{
	DataSpecification dataSpecification = await database.DataSpecifications.SingleAsync(d => d.Id == 1);
	DataSpecificationItem item1 = new DataSpecificationItem()
	{
		DataSpecification = dataSpecification,
		Iri = "abc",
		Label = "A B C",
		DataSpecificationId = dataSpecification.Id,
		Type = ItemType.Class
	};

	DataSpecificationItem item2 = new DataSpecificationItem()
	{
		DataSpecification = dataSpecification,
		Iri = "abc",
		Label = "A B C",
		DataSpecificationId = dataSpecification.Id,
		Type = ItemType.Class
	};

	await database.DataSpecificationItems.AddAsync(item1);
	var i = database.DataSpecificationItems.SingleOrDefault(it => it.Iri == item2.Iri && it.DataSpecificationId == item2.DataSpecificationId);
	if (i is null)
	{
		i = item2;
	}
});

app.Run();
