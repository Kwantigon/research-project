namespace BackendApi.DTO;

public class GetConversationsResponseDTO
{
	public required uint ConversationId { get; set; }
	
	public required string Title { get; set; }
}
