namespace Backend.Model;

public class SparqlResponse
{
	public required string SparqlQuery { get; set; }

	public required List<HighlightedProperty> HighlightedWords { get; set; }
}

/// <summary>
/// The property to be highlighted in the user's query.
/// </summary>
/// <param name="startIndex">The starting position of the property in the user's query</param>
/// <param name="wordsCount">Number of words that makes up the property (usually 1 or 2 words)</param>
public record HighlightedProperty(int startIndex, int wordsCount);
