using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class DataSpecificationServiceMock : IDataSpecificationService
{
	public DataSpecification ProcessDataspecerPackage(string dataspecerPackageIri)
	{
		string owlContent = File.ReadAllText("insurance.owl.ttl");
		return new DataSpecification
		{
			Id = 0,
			Iri = dataspecerPackageIri,
			Name = "Insurance",
			Owl = owlContent
		};
	}
}
