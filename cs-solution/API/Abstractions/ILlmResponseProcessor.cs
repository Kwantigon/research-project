using BackendApi.Model;

namespace BackendApi.Abstractions;

public interface ILlmResponseProcessor
{
	object ParseMappedDataSpecProperties(string llmResponse);
}
