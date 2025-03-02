using BackendApi.Abstractions;

namespace BackendApi.RequestHandlers;

public class GetRequestsHandler(
	ILogger<GetRequestsHandler> logger,
	IDatabase database,
	IPromptConstructor promptConstructor,
	ILlmConnector llmConnector,
	ILlmResponseProcessor llmResponseProcessor
)
{
	private readonly ILogger<GetRequestsHandler> _logger = logger;

	private readonly IDatabase _database = database;

	private readonly IPromptConstructor _promptConstructor = promptConstructor;

	private readonly ILlmConnector _llmConnector = llmConnector;

	private readonly ILlmResponseProcessor _llmResponseProcessor = llmResponseProcessor;
}
