using System.Text.Json.Serialization;

namespace DataSpecificationNavigationBackend.Model;

public class Message
{
	public Guid Id { get; set; }

	public Source Sender { get; set; }

	public string TextContent { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; }

	public required virtual Conversation Conversation { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter<Source>))]
	public enum Source
	{
		System,
		User
	}
}
