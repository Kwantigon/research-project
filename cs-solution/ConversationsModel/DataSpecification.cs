namespace ConversationsModel;

public class DataSpecification
{
	private static uint _nextUnusedId = 0;

	public DataSpecification(string dataSpecificationName)
	{
		Id = _nextUnusedId++;
		Name = dataSpecificationName;
	}

	public uint Id { get; set; }

	public string Name { get; set; }
}
