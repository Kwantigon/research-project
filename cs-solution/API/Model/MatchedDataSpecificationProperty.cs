namespace Backend.Model;

public class MatchedDataSpecificationProperty
{
	/// <summary>
	/// The unique ID under which the property is saved.
	/// I don't know if it'll be a string or something else.
	/// </summary>
	public required string PropertyId { get; set; }

	/// <summary>
	/// The index where the property starts in the user's query original query.
	/// </summary>
	public int StartingIndex { get; set; }

	/// <summary>
	/// The length of the property in the user's original query.
	/// </summary>
	public int Length { get; set; }
}
