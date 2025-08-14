using DataSpecificationNavigatorBackend.Model;
using System.Diagnostics.CodeAnalysis;

namespace DataSpecificationNavigatorBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	Task<DataSpecification?> ExportDataSpecificationFromDataspecerAsync(string dataspecerPackageUuid, string dataspecerPackageName);
}
