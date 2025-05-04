using DataspecNavigationHelper.Model;

namespace DataspecNavigationHelper.BusinessCoreLayer.DTO;

public record ConversationDTO(string Iri, string Title, int MessageCount)
{
	public static implicit operator ConversationDTO(Conversation conversation)
	{
		return new ConversationDTO($"/conversations/{conversation.Id}", conversation.Title, conversation.Messages.Count);
	}
}
