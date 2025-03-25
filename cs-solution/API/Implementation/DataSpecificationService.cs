using Backend.Abstractions;
using Backend.Model;

namespace Backend.Implementation;

public class DataSpecificationService : IDataSpecificationService
{
	public DataSpecification CreateDataSpecificationFromDataspecerPackage(string dataspecerPackageIri)
	{
		return new DataSpecification(null, dataspecerPackageIri);
	}
}
