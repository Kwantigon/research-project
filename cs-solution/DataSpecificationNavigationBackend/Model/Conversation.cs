namespace DataspecNavigationHelper.Model;

public class Conversation
{
	public int Id { get; set; } // This should be automatically assigned by the database.

	public string Title { get; set; } = "Unnamed conversation";

	public required DataSpecification DataSpecification { get; set; }

	public List<Message> Messages { get; set; } = [];

	// I will define the rest later to save time now.
	// Now I want to define other things.
}
