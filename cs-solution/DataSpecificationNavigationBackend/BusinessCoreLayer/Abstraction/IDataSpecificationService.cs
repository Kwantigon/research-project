using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	Task<DataSpecification> ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName);
	DataSpecification? GetDataSpecificationByIri(string dataSpecificationIri);
}
