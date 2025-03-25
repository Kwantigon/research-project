using Backend.DTO;

namespace Backend.Model;

public class DataSpecification
{
	private static uint _nextUnusedId = 0;

	public DataSpecification(string? dataSpecificationName, string dataspecerIri)
	{
		Id = _nextUnusedId++;
		Name = dataSpecificationName != null ? dataSpecificationName : $"Unnamed data specification #{Id}";
		DataspecerIri = dataspecerIri;
	}

	public uint Id { get; }

	public string Name { get; set; }

	public string DataspecerIri { get; set; }

	public List<DataSpecificationItem> Items { get; set; } = [];
}
