using BackendApi.Model;

namespace BackendApi.Abstractions;

public interface ILlmConnector
{
	string SendPromptAndReceiveResponse(string prompt);
}
