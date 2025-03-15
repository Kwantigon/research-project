namespace Backend.DTO;

public DataSpecificationDTO
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
}