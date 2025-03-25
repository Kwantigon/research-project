using System.Net;

namespace Backend.DTO;

public class ErrorResponseDTO
{
	// ToDo: Error code does not seem necessary when it's already in the header.
	public HttpStatusCode? ErrorCode { get; set; }

	public string? ErrorMessage { get; set; }
}
