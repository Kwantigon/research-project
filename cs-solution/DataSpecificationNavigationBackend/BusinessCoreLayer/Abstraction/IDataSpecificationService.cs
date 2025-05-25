using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	DataSpecification ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName);
}
