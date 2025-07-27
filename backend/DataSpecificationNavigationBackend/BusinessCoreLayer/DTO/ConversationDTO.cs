using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

public record ConversationDTO(int Id, string Title, string DataSpecificationName, DateTime LastUpdated)
{
	public static explicit operator ConversationDTO(Conversation conversation)
	{
		return new ConversationDTO(
			conversation.Id,
			conversation.Title,
			conversation.DataSpecification.Name,
			conversation.LastUpdated
		);
	}
}
