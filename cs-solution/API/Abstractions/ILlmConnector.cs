using Backend.Model;

namespace Backend.Abstractions;

public interface ILlmConnector
{
	string SendPromptAndReceiveResponse(string prompt);
}
