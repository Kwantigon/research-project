using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	DataSpecification ProcessDataspecerPackage(string dataspecerPackageIri);
}
