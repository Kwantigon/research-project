using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.DTO;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

/// <summary>
/// Converts the results of <see cref="IDataSpecificationService""/> to HTTP responses.
/// </summary>
public class DataSpecificationController(
	IDataSpecificationService dataSpecificationService) : IDataSpecificationController
{
	private readonly IDataSpecificationService _dataSpecificationService = dataSpecificationService;

	public async Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload)
	{
		DataSpecification dataSpecification = await _dataSpecificationService.ExportDataSpecificationFromDataspecer(payload.DataspecerPackageIri, payload.Name);
		return Results.Created($"/data-specifications/{dataSpecification.Id}", (DataSpecificationDTO)dataSpecification);
	}
}
