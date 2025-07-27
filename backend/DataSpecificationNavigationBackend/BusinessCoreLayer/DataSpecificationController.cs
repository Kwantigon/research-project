using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;
using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IDataSpecificationService""/> to HTTP responses.
/// </summary>
public class DataSpecificationController(
	ILogger<DataSpecificationController> logger,
	IDataSpecificationService dataSpecificationService) : IDataSpecificationController
{
	private readonly ILogger<DataSpecificationController> _logger = logger;
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;

	public async Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload)
	{
		_logger.LogTrace("Payload of type PostDataSpecificationsDTO: {Payload}", payload);

		_logger.LogTrace("Exporting the Dataspecer package.");
		DataSpecification? dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecerAsync(payload.DataspecerPackageUuid, payload.Name);
		if (dataSpecification is null)
		{
			return Results.InternalServerError(new ErrorDTO() { Reason = "There was an error while retrieving and processing the Dataspecer package." });
		}
		return Results.Created($"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
	}

	public async Task<IResult> GetItemSummaryAsync(int dataSpecificationId, string itemIri)
	{
		itemIri = Uri.UnescapeDataString(itemIri);
		/*_logger.LogTrace("Getting a summary for the item with IRI = \"{Iri}\" from the data specification with ID = {DataSpecId}", itemIri, dataSpecificationId);

		_logger.LogTrace("Searching for the data specification with ID={Id}.", dataSpecificationId);
		DataSpecification? dataSpecification = await _dataSpecificationService.GetDataSpecificationAsync(dataSpecificationId);
		if (dataSpecification is null)
		{
			_logger.LogError("Data specification with ID={Id} does not exist.", dataSpecificationId);
			return Results.NotFound(new ErrorDTO() { Reason = $"Data specification with ID {dataSpecificationId} not found." });
		}

		_logger.LogTrace("Searching for the item with IRI \"{Iri}\"", itemIri);
		DataSpecificationItem? item = await _dataSpecificationService.GetDataSpecificationItem(dataSpecificationId, itemIri);
		if (item is null)
		{
			_logger.LogError("Item with IRI = \"{Iri}\" does not exist.", itemIri);
			return Results.NotFound(new ErrorDTO() { Reason = $"Item {itemIri} not found." });
		}

		if (item.Summary is null)
		{
			_logger.LogTrace("Item summary is null. It will be generated.");
			await _dataSpecificationService.GenerateItemSummaryAsync(item);
			if (item.Summary is null)
			{
				_logger.LogError("Item summary is still null after the generation method finished.");
				return Results.InternalServerError(new ErrorDTO() { Reason = $"Failed to generate a summary for item {item.Label}" });
			}
		} else
		{
			_logger.LogTrace("Item summary is already present - returning it.");
		}

		return Results.Ok(new ItemSummaryDTO(item.Summary));*/
		_logger.LogDebug("Getting a summary for the item with IRI = \"{Iri}\" from the data specification with ID = {DataSpecId}", itemIri, dataSpecificationId);
		await Task.Delay(2000);
		_logger.LogTrace("Returning a mock item summary.");
		return Results.Ok(new ItemSummaryDTO("Mock item sumary. AAAAAABBBBBBBBBCCCCCCCCC."));
	}
}
