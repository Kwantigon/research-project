namespace DataSpecificationNavigationBackend.Model;

public class ReplyMessage : Message
{
	public bool IsGenerated { get; set; } = false;

	public string MappingText { get; set; } = string.Empty;

	public string SparqlText { get; set; } = string.Empty;

	public string SparqlQuery { get; set; } = string.Empty;

	public string SuggestItemsText { get; set; } = string.Empty;

	public virtual List<DataSpecificationItemSuggestion> ItemSuggestionsTable { get; set; } = [];
}
