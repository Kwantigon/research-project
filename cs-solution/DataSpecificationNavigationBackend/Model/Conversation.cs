namespace DataspecNavigationHelper.Model;

public class Conversation
{
	public int Id { get; set; }

	public string Title { get; set; } = "Unnamed conversation";

	public DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; } = [];

	public DateTime LastUpdated { get; set; }
}
