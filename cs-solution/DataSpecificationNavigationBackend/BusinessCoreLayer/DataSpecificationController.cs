using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

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
		DataSpecification dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecer(payload.DataspecerPackageIri, payload.Name);
		return Results.Created($"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
	}

	public async Task<IResult> AddEfTestDataSpecification(PostDataSpecificationsDTO payload)
	{
		DataSpecification dataSpecification = new DataSpecification()
		{
			Name = (payload.Name != null ? payload.Name : "Unnamed data specification"),
			Iri = payload.DataspecerPackageIri,
			Owl = "mock OWL value"
		};
		await _appDbContext.DataSpecifications.AddAsync(dataSpecification);
		await _appDbContext.SaveChangesAsync();
		return Results.Created($"/data-specifications/{dataSpecification.Id}", dataSpecification);
	}
}
