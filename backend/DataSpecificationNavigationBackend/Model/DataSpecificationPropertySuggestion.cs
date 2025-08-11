using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSpecificationNavigationBackend.Model;

[PrimaryKey(nameof(ItemDataSpecificationId), nameof(ItemIri), nameof(ReplyMessageId))]
public class DataSpecificationPropertySuggestion
{
	public int ItemDataSpecificationId { get; set; }

	public required string ItemIri { get; set; }

	public Guid ReplyMessageId { get; set; }

	public virtual required DataSpecificationItem Item { get; set; }

	public virtual required ReplyMessage ReplyMessage { get; set; }

	public required string ReasonForSuggestion { get; set; }

	public required string DomainItemIri { get; set; }

	public virtual required DataSpecificationItem DomainItem { get; set; }

	public required string RangeItemIri { get; set; }

	// Not mapped because RangeItemIri could just be a data type.
	[NotMapped]
	public DataSpecificationItem? RangeItem { get; set; }
}
