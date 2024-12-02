using RequestHandler;
using Microsoft.AspNetCore.Mvc;
using DTO;

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
var app = builder.Build();
app.UseCors();

#region GET requests
// Sanity check.
app.MapGet("/", () => "Hello there!");

// Return a list of IDs of previous chats.
app.MapGet("/previous-conversations", () => Handler.GET.PreviousConversations());

// Returns all messages from the conversation with 'conversationId'.
app.MapGet("/conversationHistory/{conversationId}", ([FromRoute] uint conversationId) => Handler.GET.ConversationHistory(conversationId));

// Returns properties, which can be used to expand the user's query.
// MAYBE?: also return the userQuery with all the added properties.
	// When the frontend needs to show preview of the expanded query, it can use the returned modified userQuery.
	// I think that might be too hard to do. Because there could be some nesting when users display the available properties.
app.MapGet("/query-expansion-properties", ([FromQuery(Name = "userQuery")] string userQuery) => Handler.GET.QueryExpansionProperties(userQuery));
// ToDo: Is it a good idea to pass the user's query as a string in the query part of the URL?

// Returns the summary for one selected property.
app.MapGet("/property-summary", ([FromQuery(Name = "propertyName")] string propertyName) => Handler.GET.PropertySummary(propertyName));
#endregion

#region POST requests
app.MapPost("/data-specification", ([FromBody] DataSpecificationUrl dataSpecUrl) => Handler.POST.DataSpecification(dataSpecUrl));

// This should simply translate the user's initial query to Sparql.
// Returns a Sparql query in the response.
app.MapPost("/user-initial-query", ([FromBody] UserQuery initialQuery) => Handler.POST.UserInitialQuery(initialQuery));

// Very similar to POST /user-initial-query.
// Also returns a Sparql query as a response.
// This name is not very good because I immediately forgot what the resource is supposed to do.
// This resource should translate the user's expanded query to Sparql.
app.MapPost("/user-expanded-query", ([FromBody] UserQuery expandedQuery) => Handler.POST.UserExpandedQuery(expandedQuery));

// The previous 2 POST endpoints could possibly be merged into this.
app.MapPost("/translate-to-sparql", () => { });

// NOTE: possibly merge user-initial-query and user-expanded-query?
// ToDo: Analyze further, if processing is identical, merge them.
#endregion

#region PUT requests
app.MapPut("/expand-query", (ExpandQueryDTO expandQueryDTO) => Handler.PUT.ExpandQuery(expandQueryDTO));
#endregion

app.Run();
