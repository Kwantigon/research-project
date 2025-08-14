using DataSpecificationNavigationBackend.Model;
using System.Diagnostics.CodeAnalysis;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	Task<DataSpecification?> ExportDataSpecificationFromDataspecerAsync(string dataspecerPackageUuid, string dataspecerPackageName);
}
