namespace Backend.Model;

public class DataSpecification
{
	private static uint _nextUnusedId = 0;

	public DataSpecification(string? dataSpecificationName, string dataspecerUri)
	{
		Id = _nextUnusedId++;
		Name = dataSpecificationName != null ? dataSpecificationName : $"Unnamed data specification #{Id}";
		DataspecerUri = dataspecerUri;
	}

	public uint Id { get; }

	public string Name { get; set; }

	public string DataspecerUri { get; set; }
}
