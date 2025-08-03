namespace DataSpecificationNavigationBackend.Model;

public class ReplyMessage : Message
{
	public override Source Sender { get => Source.System; }

	public bool IsGenerated { get; set; } = false;

	public required virtual UserMessage PrecedingUserMessage { get; set; }

	public string MappingText { get; set; } = string.Empty;

	public string SparqlText { get; set; } = string.Empty;

	public string SparqlQuery { get; set; } = string.Empty;

	public string SuggestItemsText { get; set; } = string.Empty;

	public virtual List<DataSpecificationItemSuggestion> ItemSuggestions { get; set; } = [];

}
