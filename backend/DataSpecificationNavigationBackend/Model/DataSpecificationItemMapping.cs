using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(ItemDataSpecificationId), nameof(ItemIri), nameof(UserMessageId))]
public class DataSpecificationItemMapping
{
	public int ItemDataSpecificationId { get; set; }

	public required string ItemIri { get; set; }

	public Guid UserMessageId { get; set; }

	public virtual required DataSpecificationItem Item { get; set; }

	public virtual required UserMessage UserMessage { get; set; }

	public required string MappedWords { get; set; }

	public bool IsSelectTarget { get; set; } = false;
}
