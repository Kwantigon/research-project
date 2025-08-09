using System.ComponentModel.DataAnnotations.Schema;

namespace DataSpecificationNavigationBackend.Model;

public class Conversation
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public DateTime LastUpdated { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }

	// !Important: Always retrieve the messages sorted by their timestamps.
	public virtual List<Message> Messages { get; set; } = [];

	public virtual DataSpecificationSubstructure DataSpecificationSubstructure { get; set; } = new();

	/// <summary>
	/// IRIs of the items that user has selected for question expansion.
	/// </summary>
	public List<string> UserSelectedItems { get; set; } = [];

	public string? SuggestedMessage { get; set; }
}
