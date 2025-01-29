namespace BackendApi.Model;

public class Conversation
{
	private static uint _nextUnusedId = 0;

	public Conversation(DataSpecification dataSpecification, string? title)
	{
		Id = _nextUnusedId++;
		Title = title != null ? title : $"Unnamed conversation #{Id}";
		DataSpecification = dataSpecification;
		Messages = new List<Message>();
	}

	public uint Id { get; set; }

	public string Title { get; set; }

	public DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; }
}
