namespace BackendApi.Model;

public class Conversation
{
	private static uint _nextUnusedId = 0;

	private uint _nextUnusedMessageId = 0;

	public Conversation(DataSpecification dataSpecification, string? title)
	{
		Id = _nextUnusedId++;
		Title = title != null ? title : $"Untitled conversation #{Id}";
		DataSpecification = dataSpecification;
		Messages = new List<Message>();
	}

	public uint Id { get; }

	public string Title { get; set; }

	public DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; }

	public DataSpecificationSubstructure? DataSpecificationSubstructure { get; set; }
}
