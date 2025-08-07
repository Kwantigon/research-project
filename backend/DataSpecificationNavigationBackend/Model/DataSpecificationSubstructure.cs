namespace DataSpecificationNavigationBackend.Model;

public class DataSpecificationSubstructure
{
	public List<ClassItem> Targets { get; set; } = [];
	public List<ClassItem> NonTargets { get; set; } = [];

	public class ClassItem
	{
		public string Iri { get; set; }

		public string Label { get; set; }

		public List<PropertyItem> ObjectProperties { get; set; } = [];

		public List<PropertyItem> DatatypeProperties { get; set; } = [];
	}

	public class PropertyItem
	{
		public string Iri { get; set; }

		public string Label { get; set; }

		public string Domain { get; set; }

		public string Range { get; set; }
	}
}
