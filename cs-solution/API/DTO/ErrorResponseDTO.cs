using System.Net;

namespace Backend.DTO;

public class ErrorResponseDTO
{
	public HttpStatusCode? ErrorCode { get; set; }

	public string? ErrorMessage { get; set; }
}
