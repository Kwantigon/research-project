
namespace Backend.DTO
{
	public class PostDataSpecificationsRequestDTO
	{
		/// <summary>
		/// The IRI to the Dataspecer package.
		/// </summary>
		public string? IriToDataspecer { get; set; }

		/// <summary>
		/// The name to store this data specification under.
		/// </summary>
		public string? Name { get; set; }
	}
}
