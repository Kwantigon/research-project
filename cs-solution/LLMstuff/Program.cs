using Implementations;
using Abstractions;

/*ILanguageModelConnector llm = new OpenAIConnector();
string response = llm.SendPrompt("Hello");
Console.WriteLine(response);*/

ILanguageModelConnector dummyLlm = new DummyLlmConnector();
Console.WriteLine(dummyLlm.SendPrompt("Hello"));