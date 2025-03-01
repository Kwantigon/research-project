
namespace BackendApi.DTO
{
	public class PostDataSpecificationsRequestDTO
	{
		/// <summary>
		/// The URI to the Dataspecer package.
		/// </summary>
		public required string UriToDataspecer { get; set; }

		/// <summary>
		/// The name to store this data specification under.
		/// </summary>
		public string? Name { get; set; }
	}
}
