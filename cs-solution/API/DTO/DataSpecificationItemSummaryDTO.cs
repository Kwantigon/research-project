namespace Backend.DTO;

public class DataSpecificationItemSummaryDTO
{
	public required string TextualSummary { get; set; }

	/// <summary>
	/// Contains IRIs of items that are relevant to the summary.
	/// Sending a GET request to these IRIs will return a summary for each of those items.
	/// </summary>
	public required List<string> LocationsOfItems { get; set; }
}
