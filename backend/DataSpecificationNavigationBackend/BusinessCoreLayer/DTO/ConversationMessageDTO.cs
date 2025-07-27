using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

public record ConversationMessageDTO(Guid Id, MessageType Type, string Text, List<DataSpecificationItemDTO>? RelatedItems, DateTime Timestamp, string? ReplyUri)
{
	public static explicit operator ConversationMessageDTO(Message message)
	{
		string? replyUri = message.ReplyMessageId == null ? null : $"/conversations/{message.ConversationId}/messages/{message.ReplyMessageId}";
		List<DataSpecificationItemDTO>? relatedItems = message.RelatedItems?.Select(item => (DataSpecificationItemDTO)item).ToList();
		return new ConversationMessageDTO(message.Id, message.Type, message.TextValue, relatedItems, message.TimeStamp, replyUri);
	}
}
