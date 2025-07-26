using DataspecNavigationBackend.Model;

namespace DataspecNavigationBackend.BusinessCoreLayer.DTO;

public record DataSpecificationDTO(string BackendIri, string Name, string DataspecerIri)
{
	public static implicit operator DataSpecificationDTO(DataSpecification dataSpecification)
	{
		return new DataSpecificationDTO($"/data-specifications/{dataSpecification.Id}", dataSpecification.Name, dataSpecification.DataspecerPackageUuid);
	}
}
