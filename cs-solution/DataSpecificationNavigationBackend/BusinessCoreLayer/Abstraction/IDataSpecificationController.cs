using DataspecNavigationBackend.BusinessCoreLayer.DTO;

namespace DataspecNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationController
{
	Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload);


	//Task<IResult> AddEfTestDataSpecification(PostDataSpecificationsDTO payload);
}
