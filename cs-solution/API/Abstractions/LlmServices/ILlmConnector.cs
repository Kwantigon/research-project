namespace Backend.Abstractions.LlmServices;

public interface ILlmConnector
{
	string SendPromptAndReceiveResponse(string prompt);
}
