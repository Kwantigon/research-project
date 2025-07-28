using DataSpecificationNavigationBackend.Model;
using System.Diagnostics.CodeAnalysis;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;

public interface IDataSpecificationService
{
	Task<DataSpecification?> ExportDataSpecificationFromDataspecerAsync(string dataspecerPackageUuid, string dataspecerPackageName);
	Task<DataSpecification?> GetDataSpecificationAsync(int dataSpecificationId);
	Task<DataSpecificationItem?> GetDataSpecificationItemAsync(int dataSpecificationId, string itemIri);
	Task GenerateItemSummaryAsync(DataSpecificationItem item);
	Task<List<DataSpecificationItem>> GetItemsByIriListAsync(int dataSpecificationId, List<string> itemIriList);
}
