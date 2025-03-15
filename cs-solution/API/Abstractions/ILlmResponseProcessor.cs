using Backend.Model;

namespace Backend.Abstractions;

public interface ILlmResponseProcessor
{
	object ParseMappedDataSpecProperties(string llmResponse);
}
