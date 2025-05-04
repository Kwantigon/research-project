namespace DataspecNavigationHelper.Model;

public class Conversation
{
	public int Id { get; set; }

	public string Title { get; set; } = "Unnamed conversation";

	public List<Message> Messages { get; set; } = [];
	// I will define the rest later to save time now.
	// Now I want to define other things.
}
