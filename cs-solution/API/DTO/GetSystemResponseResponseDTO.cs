using BackendApi.Model;

namespace BackendApi.DTO;

public class GetSystemResponseResponseDTO
{
	public required PositiveSystemMessage SystemAnswer { get; set; }
}
