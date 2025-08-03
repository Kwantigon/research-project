using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using System.Text;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class PromptConstructor : IPromptConstructor
{
	/// <summary>
	/// Has the following parameters:<br/>
	/// {0} = Data specification (OWL file).<br/>
	/// {1} = User question.
	/// </summary>
	private readonly string _itemsMappingTemplate;

	/// <summary>
	/// Has the following parameters:<br/>
	/// {0} = Data specification (OWL file).<br/>
	/// {1} = User question.<br/>
	/// {2} = Current substructure (a list of items that the conversation has built).
	/// </summary>
	private readonly string _getRelatedItemsTemplate;

	/// <summary>
	/// Has the following parameters:<br/>
	/// {0} = Data specification (OWL file).<br/>
	/// {1} = User question.<br/>
	/// {2} = Current substructure (a list of items that the conversation has built).<br/>
	/// {3} = Selected items (a list of items that the user has selected to add to their question).
	/// </summary>
	private readonly string _generateSuggestedMessageTemplate;

	/// <summary>
	/// Has the following parameters:<br/>
	/// {0} = Data specification (OWL file).<br/>
	/// {1} = User question.<br/>
	/// {2} = Current substructure (a list of items that the conversation has built).
	/// </summary>
	private readonly string _dataSpecSubstructureItemsMappingTemplate;

	public PromptConstructor(IConfiguration appSettings)
	{
		string? baseDirectory = appSettings["Prompts:BaseDirectory"];
		if (baseDirectory is null)
		{
			throw new Exception("The key Prompts:BaseDirectory is missing in the config file.");
		}

		string? itemsMapping = appSettings["Prompts:ItemsMapping"];
		if (itemsMapping is null)
		{
			throw new Exception("The key Prompts:ItemsMapping is missing in the config file.");
		}
		string file = Path.Combine(baseDirectory, itemsMapping);
		if (!File.Exists(file))
		{
			throw new Exception($"The template file \"{file}\" does not exist");
		}
		else
		{
			_itemsMappingTemplate = File.ReadAllText(file);
		}

		string? getRelatedItems = appSettings["Prompts:GetRelatedItems"];
		if (getRelatedItems is null)
		{
			throw new Exception("The key Prompts:GetRelatedItems is missing in the config file.");
		}
		file = Path.Combine(baseDirectory, getRelatedItems);
		if (!File.Exists(file))
		{
			throw new Exception($"The template file \"{file}\" does not exist");
		}
		else
		{
			_getRelatedItemsTemplate = File.ReadAllText(file);
		}

		string? generateSuggestedMessage = appSettings["Prompts:GenerateSuggestedMessage"];
		if (generateSuggestedMessage is null)
		{
			throw new Exception("The key Prompts:GenerateSuggestedMessage is missing in the config file.");
		}
		file = Path.Combine(baseDirectory, generateSuggestedMessage);
		if (!File.Exists(file))
		{
			throw new Exception($"The template file \"{file}\" does not exist");
		}
		else
		{
			_generateSuggestedMessageTemplate = File.ReadAllText(file);
		}

		string? substructureItemMapping = appSettings["Prompts:DataSpecSubstructureItemsMapping"];
		if (substructureItemMapping is null)
		{
			throw new Exception("The key Prompts:DataSpecSubstructureItemsMapping is missing in the config file.");
		}
		file = Path.Combine(baseDirectory, substructureItemMapping);
		if (!File.Exists(file))
		{
			throw new Exception($"The template file \"{file}\" does not exist");
		}
		else
		{
			_dataSpecSubstructureItemsMappingTemplate = File.ReadAllText(file);
		}
	}

	public string BuildItemsMappingPrompt(DataSpecification dataSpecification, string userQuestion)
	{
		return string.Format(_itemsMappingTemplate, dataSpecification.Owl, userQuestion);
	}

	public string BuildGetSuggestedItemsPrompt(DataSpecification dataSpecification, string userQuestion, List<DataSpecificationItem> mappedItems)
	{
		StringBuilder currentSubstructure = new();
		foreach (DataSpecificationItem item in mappedItems)
		{
			currentSubstructure.AppendLine($"- {item.Iri}");
		}

		return string.Format(_getRelatedItemsTemplate, dataSpecification.Owl, userQuestion, currentSubstructure.ToString());
	}

	public string BuildGenerateSuggestedMessagePrompt(DataSpecification dataSpecification, string userQuestion, List<DataSpecificationItem> mappedItems, List<DataSpecificationItem> selectedItems)
	{
		StringBuilder currentSubstructure = new();
		foreach (DataSpecificationItem item in mappedItems)
		{
			currentSubstructure.AppendLine($"- {item.Iri}");
		}

		StringBuilder selected = new();
		foreach (DataSpecificationItem item in selectedItems)
		{
			selected.AppendLine($"- {item.Iri}");
		}

		return string.Format(_generateSuggestedMessageTemplate, dataSpecification.Owl, userQuestion, currentSubstructure.ToString(), selected.ToString());
	}

	public string BuildDataSpecSubstructureItemsMappingPrompt(DataSpecification dataSpecification, string userQuestion, List<DataSpecificationItem> dataSpecificationSubstructure)
	{
		StringBuilder currentSubstructure = new();
		foreach (DataSpecificationItem item in dataSpecificationSubstructure)
		{
			currentSubstructure.AppendLine($"- {item.Iri}");
		}

		return string.Format(_dataSpecSubstructureItemsMappingTemplate, dataSpecification.Owl, userQuestion, currentSubstructure.ToString());
	}
}
