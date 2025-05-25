using DataspecNavigationHelper.BusinessCoreLayer.DTO;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationController
{
	IResult ProcessDataspecerPackage(PostDataSpecificationsDTO payload);
}
