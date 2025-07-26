using DataspecNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

public record ConversationMessageDTO(Guid Id, MessageType Type, string Text, List<DataSpecificationItem>? RelatedItems, DateTime Timestamp, string? ReplyUri)
{
	public static explicit operator ConversationMessageDTO(Message message)
	{
		string? replyUri = message.ReplyMessageId == null ? null : $"/conversations/{message.ConversationId}/messages/{message.ReplyMessageId}";
		return new ConversationMessageDTO(message.Id, message.Type, message.TextValue, message.RelatedItems, message.TimeStamp, replyUri);
	}
}
