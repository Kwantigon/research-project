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
		_logger.LogDebug("Payload of type PostDataSpecificationsDTO: {Payload}", payload);

		_logger.LogTrace("Exporting the Dataspecer package.");
		DataSpecification? dataSpecification =
			await _dataSpecificationService.ExportDataSpecificationFromDataspecerAsync(payload.DataspecerPackageUuid, payload.Name);
		if (dataSpecification is null)
		{
			_logger.LogError("Failed to export the Dataspecer package with UUID {Uuid} and name {Name}.", payload.DataspecerPackageUuid, payload.Name);
			return Results.InternalServerError(new ErrorDTO() { Reason = "There was an error while retrieving and processing the Dataspecer package." });
		}

		return Results.Created($"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
	}
}
