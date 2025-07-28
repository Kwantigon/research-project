using System.ComponentModel.DataAnnotations.Schema;

namespace DataSpecificationNavigationBackend.Model;

public class Conversation
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public int DataSpecificationId { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }

	// !Important: Always retrieve the messages sorted by their timestamps.
	public virtual List<Message> Messages { get; set; } = [];

	public DateTime LastUpdated { get; set; }

	// Todo: This should be a set rather than a List.
	public virtual List<DataSpecificationItem> DataSpecificationSubstructure { get; set; } = [];

	/// <summary>
	/// IRIs of the selected items.
	/// Using List&lt;DataSpecificationItem&gt; would interfere with the implicit configuration of Entity Framework.
	/// And I think I don't strictly need to store it as the items themselves anyway. IRI should be enough.
	/// </summary>
	public List<string>? UserSelectedItems { get; set; }
}
