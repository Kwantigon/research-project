using Backend.Model;

namespace Backend.DTO;

public class DataSpecificationDTO
{
	/// <summary>
	/// The name of the data specification.
	/// This was either given by the user or it is a default name created by the server.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The IRI of the data specification on the server.
	/// </summary>
	public required string Location { get; set; }

	/// <summary>
	/// The IRI of the original Dataspecer package.
	/// </summary>
	public required string DataspecerIri { get; set; }

	public static explicit operator DataSpecificationDTO(DataSpecification dataSpecification)
	{
		return new DataSpecificationDTO
		{
			Name = dataSpecification.Name,
			Location = $"/data-specifications/{dataSpecification.Id}",
			DataspecerIri = dataSpecification.DataspecerIri
		};
	}
}