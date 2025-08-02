using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class DefaultResponseProcessor(
	ILogger<DefaultResponseProcessor> logger,
	AppDbContext appDbContext) : ILlmResponseProcessor
{
	private readonly ILogger<DefaultResponseProcessor> _logger = logger;
	private readonly AppDbContext _database = appDbContext;

	public List<DataSpecificationItemMapping>? ExtractMappedItems(string llmResponse, Conversation conversation)
	{
		throw new NotImplementedException();
	}
}
