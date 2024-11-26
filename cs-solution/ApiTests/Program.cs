using ApiTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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

app.MapGet("/", () => "Hello World!");
app.MapPost("/prompt", ([FromQuery(Name = "value")] string prompt) => Handlers.PostUserPrompt(prompt));
app.MapGet("/chat-history/{chatId}", (int chatId) => Handlers.GetChatHistory(chatId));

app.Run();
