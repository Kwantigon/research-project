namespace DataspecNavigationHelper.Model;

public class DataSpecificationItem
{
	public required string Iri { get; init; }

	/// <summary>
	/// Corresponds to the label of the class or property in the OWL file.
	/// </summary>
	public required string Name { get; init; }

	public required ItemType Type { get; init; }

	public required DataSpecification DataSpecification { get; init; }
}

public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DataProperty // Attributes of entities.
}
