namespace BackendApi.Model;

public class PropertySummary
{
	public required string TextualSummary { get; set; }

	/// <summary>
	/// IDs of the related properties. The front end might need to send GET requests for their summaries.
	/// </summary>
	public List<string> RelatedProperties { get; set; } = [];
}
