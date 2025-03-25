using Backend.Model;

namespace Backend.Abstractions;

public interface IDataSpecificationService
{
	DataSpecification CreateDataSpecificationFromDataspecerPackage(string dataspecerPackageIri);
}
