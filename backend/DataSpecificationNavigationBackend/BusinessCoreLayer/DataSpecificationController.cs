using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.BusinessCoreLayer.Abstraction;
using DataspecNavigationBackend.BusinessCoreLayer.DTO;
using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IDataSpecificationService""/> to HTTP responses.
/// </summary>
public class DataSpecificationController(
	IDataSpecificationService dataSpecificationService,
	AppDbContext appDbContext) : IDataSpecificationController
{
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;
	// Only for mocking purposes.
	AppDbContext _appDbContext = appDbContext;

	public async Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload)
	{
		DataSpecification? dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecerAsync(payload.DataspecerPackageUuid, payload.Name);
		if (dataSpecification is null)
		{
			return Results.InternalServerError(new Error() { Reason = "There was an error while retrieving and processing the Dataspecer package." });
		}
		return Results.Created($"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
	}

	/*public async Task<IResult> AddEfTestDataSpecification(PostDataSpecificationsDTO payload)
	{
		DataSpecification dataSpecification = new DataSpecification(0, payload.DataspecerPackageUuid, payload.Name, "Mock OWL value");
		await _appDbContext.DataSpecifications.AddAsync(dataSpecification);
		await _appDbContext.SaveChangesAsync();
		return Results.Created($"/data-specifications/{dataSpecification.Id}", dataSpecification);
	}*/
}
