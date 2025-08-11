using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(ItemDataSpecificationId), nameof(ItemIri), nameof(UserMessageId))]
public class DataSpecificationItemMapping
{
	public required string ItemIri { get; set; }

	public required string MappedWords { get; set; }

	public bool IsSelectTarget { get; set; } = false;

	public int ItemDataSpecificationId { get; set; }

	public Guid UserMessageId { get; set; }

	public required virtual DataSpecificationItem Item { get; set; }

	public required virtual UserMessage UserMessage { get; set; }
}
