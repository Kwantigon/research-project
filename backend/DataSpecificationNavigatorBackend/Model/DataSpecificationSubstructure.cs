namespace DataSpecificationNavigatorBackend.Model;

public class DataSpecificationSubstructure
{
	public List<ClassItem> ClassItems { get; set; } = [];
	/*
	 * Each ClassItem in this list should have unique IRI, meaning each item is in there only once.
	 * I'm not using a HashSet because the uniqueness is not bound to the instance of the ClassItem,
	 * but rather to its IRI.
	 * HashSet does not help if I create two instances with identical values and add them.
	 */

	public class ClassItem
	{
		public required string Iri { get; set; }

		public required string Label { get; set; }

		public List<PropertyItem> ObjectProperties { get; set; } = [];

		public List<PropertyItem> DatatypeProperties { get; set; } = [];

		public required bool IsSelectTarget { get; set; } = false;
	}

	public class PropertyItem
	{
		public required string Iri { get; set; }
					 
		public required string Label { get; set; }
					 
		public required string Domain { get; set; }
					 
		public required string Range { get; set; }
	}
}
