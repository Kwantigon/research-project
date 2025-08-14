using DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationController
{
	Task<IResult> ProcessDataspecerPackage(PostDataSpecificationsDTO payload);
}
