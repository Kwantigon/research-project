using DataspecNavigationBackend.BusinessCoreLayer.DTO;

namespace DataspecNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationController
{
	Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload);

	Task<IResult> GetItemSummaryAsync(int dataSpecificationId, string itemIri);
}
