namespace DataSpecificationNavigationBackend.Model;

public class Conversation
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public int DataSpecificationId { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }

	public virtual List<Message> Messages { get; set; } = [];

	public DateTime LastUpdated { get; set; }
}
