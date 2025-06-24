using LLMstuff;
using System.Text.Json;


/*string dataSpecification = File.ReadAllText("./dsv.ttl");
GeminiConnector geminiConnector = new GeminiConnector();
geminiConnector.Initialize();
List<string> items = geminiConnector.MapQuestionToItems(dataSpecification, "Chci vidět záznamy s identifikátorem ISVS.");
Console.WriteLine(items[0]);*/

const string jsonResponse = """
	[
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#záznam-informacního-systému-nahrazující-úredne-overeny-podpis",
	                "type": "Class",
	                "label": "záznam informacního systému nahrazující úredne overeny podpis",
	                "comment": ""
	        },
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#je-záznamem-o",
	                "type": "ObjectProperty",
	                "label": "je záznamem o",
	                "comment": ""
	        },
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#nahrazení-úredne-overeného-podpisu-nebo-uznávaného-elektronického-podpisu",
	                "type": "Class",
	                "label": "nahrazení úredne overeného podpisu nebo uznávaného elektronického podpisu",
	                "comment": ""
	        },
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#provádí-isvs",
	                "type": "ObjectProperty",
	                "label": "provádí ISVS",
	                "comment": ""
	        },
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#informacní-systém-verejné-správy",
	                "type": "Class",
	                "label": "informacní systém verejné správy",
	                "comment": ""
	        },
	        {
	                "iri": "https://ofn.gov.cz/záznam-informacního-systému-nahrazující-úredne-overeny-podpis/draft#má-identifikátor-isvs",
	                "type": "DatatypeProperty",
	                "label": "má identifikátor ISVS",
	                "comment": ""
	        }
	]
	""";
var dataSpecificationItems = JsonSerializer.Deserialize<List<DataSpecificationItem>>(jsonResponse);
if (dataSpecificationItems is null) Console.WriteLine("NULL");
