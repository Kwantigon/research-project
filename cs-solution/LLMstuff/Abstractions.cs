namespace Abstractions
{
	public interface ILanguageModelConnector
	{
		string SendPrompt(string prompt);
	}

	public interface IResponseProcessor
	{
		/// <summary>
		/// Not sure yet what should the output should be.
		/// </summary>
		/// <param name="response">The response from the LLM. Should be in JSON according to this project's format.</param>
		void ProcessResponse(string response);
	}
}