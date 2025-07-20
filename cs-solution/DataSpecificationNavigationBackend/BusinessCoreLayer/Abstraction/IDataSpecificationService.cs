using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	Task<DataSpecification?> ExportDataSpecificationFromDataspecerAsync(string dataspecerPackageUuid, string dataspecerPackageName);
	Task<DataSpecification?> GetDataSpecificationAsync(int dataSpecificationId);
}
