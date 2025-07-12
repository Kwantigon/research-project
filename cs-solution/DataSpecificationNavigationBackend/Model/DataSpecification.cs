namespace DataspecNavigationHelper.Model;

/// <summary>
/// A Dataspecer package is used as the data specification.
/// </summary>
public class DataSpecification
{
	public int Id { get; set; }

	/// <summary>
	/// This name is the same as the Dataspecer package name.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The IRI of the Dataspecer package.
	/// </summary>
	public required string Iri { get; set; }

	/// <summary>
	/// The content of the model.owl.ttl file that represents the model as RDF triples.
	/// </summary>
	public required string Owl { get; set; }

	//public required string JsonLd { get; init; }
}
