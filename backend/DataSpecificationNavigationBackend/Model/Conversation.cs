using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace DataSpecificationNavigationBackend.Model;

public class Conversation
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public DateTime LastUpdated { get; set; }

	public required virtual DataSpecification DataSpecification { get; set; }

	// !Important: Always retrieve the messages sorted by their timestamps.
	public virtual List<Message> Messages { get; set; } = [];

	public string SubstructureJsonString { get; set; } = string.Empty; // Temporary solution using an string property

	[NotMapped]
	public DataSpecificationSubstructure DataSpecificationSubstructure
	{
		get
		{
			if (string.IsNullOrEmpty(SubstructureJsonString))
				return new DataSpecificationSubstructure();

			DataSpecificationSubstructure? substructure = JsonSerializer.Deserialize<DataSpecificationSubstructure>(SubstructureJsonString);
			if (substructure is null)
			{
				return new DataSpecificationSubstructure();
			}
			else
			{
				return substructure;
			}
		}

		set
		{
			JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
			{
				Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
				WriteIndented = true
			};
			SubstructureJsonString = JsonSerializer.Serialize(value, serializerOptions);
		}
	}

	/// <summary>
	/// IRIs of the items that user has selected for question expansion.
	/// </summary>
	public List<string> UserSelectedItems { get; set; } = [];

	public string? SuggestedMessage { get; set; }
}
