using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(ItemDataSpecificationId), nameof(ItemIri), nameof(ReplyMessageId))]
public class DataSpecificationItemSuggestion
{
	public int ItemDataSpecificationId { get; set; }

	public required string ItemIri { get; set; }

	public Guid ReplyMessageId { get; set; }

	public virtual required DataSpecificationItem Item { get; set; }

	public virtual required ReplyMessage ReplyMessage { get; set; }

	public required string ReasonForSuggestion { get; set; }

	public required string ExpandsItem { get; set; }
}
