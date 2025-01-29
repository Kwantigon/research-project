namespace BackendApi.Model;

public class DataSpecification
{
	private static uint _nextUnusedId = 0;

	public DataSpecification(string? dataSpecificationName, string dataSpecificationUri)
	{
		Id = _nextUnusedId++;
		Name = dataSpecificationName != null ? dataSpecificationName : $"Unnamed data specification #{Id}";
		Uri = dataSpecificationUri;
	}

	public uint Id { get; set; }

	public string Name { get; set; }

	public string Uri { get; set; }
}
