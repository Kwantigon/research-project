using GenerativeAI.Types;
using GenerativeAI;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace LLMstuff;

public class GeminiConnector
{
	private readonly PromptConstructor _promptConstructor = new();
	private readonly ResponseProcessor _responseProcessor = new();
	private string? _apiKey;
	private string? _model;
	private GenerativeModel? _gemini;

	[MemberNotNull(nameof(_gemini))]
	public void Initialize()
	{
		LoadLlmSettings();
		GoogleAi googleAi = new(_apiKey);
		_gemini = googleAi.CreateGenerativeModel(_model);
	}

	public List<string> MapQuestionToItems(string dataSpecification, string question)
	{
		string prompt = _promptConstructor.CreateQuestionToItemsMappingPrompt(question, dataSpecification);
		string response = SendPrompt(prompt);
		//List<DataSpecificationItem> items = _responseProcessor.MapToDataSpecificationItems(response);
		return new List<string> { response };
	}

	private string SendPrompt(string prompt)
	{
		if (_gemini is null)
		{
			throw new Exception("The Gemini model has not been initialized. Call the Initialize() method first.");
		}
		Task<GenerateContentResponse> task = _gemini.GenerateContentAsync(prompt);
		task.Wait();
		return task.Result.Text;
	}

	[MemberNotNull(nameof(_apiKey), nameof(_model))]
	private void LoadLlmSettings()
	{
		_apiKey = "AIzaSyCxQytFZxal6t-T4hfuldFfwn9NFhY06Sw";
		_model = "models/gemini-2.5-flash";
	}
}

internal class PromptConstructor
{
	const string MAP_QUESTION_TO_ITEMS_PROMPT = """
		Given an OWL file describing a data specification and a question in natural language, map the question to relevant OWL entities and return then in a JSON according to the following schema:
		[{{
			"iri": "",
			"type": "",
			"label": "",
			"comment": ""
		}}]
		Give me only the JSON array and nothing else. Do not wrap the array with backticks.

		Example output:
		[
			{{
				"iri": "https://www.example.com/item-one",
				"type": "Class",
				"label": "item one",
				"comment": ""
			}},
			{{
				"iri": "https://www.example.com/item-two",
				"type": "ObjectProperty",
				"label": "item two",
				"comment": ""
			}},
			{{
				"iri": "https://www.example.com/item-three",
				"type": "DatatypeProperty",
				"label": "item three",
				"comment": ""
			}}
		]

		The OWL file is
		```
		{0}
		```

		The question is
		```
		{1}
		```
		""";

	public string CreateQuestionToItemsMappingPrompt(string question, string dataSpecification)
	{
		return string.Format(MAP_QUESTION_TO_ITEMS_PROMPT, dataSpecification, question);
	}
}

internal class ResponseProcessor
{
	public List<DataSpecificationItem> MapToDataSpecificationItems(string llmResponse)
	{
		var dataSpecificationItems = JsonSerializer.Deserialize<List<DataSpecificationItem>>(llmResponse);
		if (dataSpecificationItems is null)
			return new List<DataSpecificationItem>();
		else
			return dataSpecificationItems;
	}
}