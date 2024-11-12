using Implementations;
using Abstractions;

ILanguageModelConnector llm = new OpenAIConnector();
string response = llm.SendPrompt("Hello");
Console.WriteLine(response);