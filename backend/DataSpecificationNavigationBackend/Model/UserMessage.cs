namespace DataSpecificationNavigationBackend.Model;

public class UserMessage : Message
{
	public Guid? ReplyMessageId { get; set; }

	public virtual ReplyMessage? ReplyMessage { get; set; }


	public virtual List<DataSpecificationItemMapping> ItemMappingsTable { get; set; } = [];
}
