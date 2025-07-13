using DataspecNavigationHelper.BusinessCoreLayer.DTO;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationController
{
	Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload);


	Task<IResult> AddEfTestDataSpecification(PostDataSpecificationsDTO payload);
}
