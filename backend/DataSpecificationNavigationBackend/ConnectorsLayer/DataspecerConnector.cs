using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using System.IO.Compression;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class DataspecerConnector(
	ILogger<DataspecerConnector> logger) : IDataspecerConnector
{
	//private const string DATASPECER_JSONLD_ENDPOINT = "https://tool.dataspecer.com/api/preview/context.jsonld?iri=";
	private const string DATASPECER_DOWNLOAD_DOCUMENTATION_ENDPOINT = "https://tool.dataspecer.com/api/experimental/output.zip?iri=";

	private readonly HttpClient _httpClient = new HttpClient();
	private readonly ILogger<DataspecerConnector> _logger = logger;

	public async Task<string?> ExportPackageDocumentation(string packageIri)
	{
		string uri = DATASPECER_DOWNLOAD_DOCUMENTATION_ENDPOINT + packageIri;
		_logger.LogTrace("Downloading the Dataspecer package documentation from the URI: {URI}.", uri);
		HttpResponseMessage response = await _httpClient.GetAsync(uri);
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Failed to download Dataspecer package documentation. Response code = {ResponseCode}", response.StatusCode);
			string body = await response.Content.ReadAsStringAsync();
			_logger.LogError("Response body:\n{Body}", body);
			return null;
		}
		byte[] data = await response.Content.ReadAsByteArrayAsync();

		/*const string zipFilePath = "./documentation.zip";
		File.WriteAllBytes(zipFilePath, data);
		_logger.LogTrace("Package documentation stored in the local file system.");*/

		_logger.LogTrace("Opening the DSV for reading.");
		const string dsvPath = "en/dsv.ttl";
		using (MemoryStream zipStream = new MemoryStream(data))
		using (ZipArchive zip = new ZipArchive(zipStream))
		{
			ZipArchiveEntry? dsvFile = zip.GetEntry(dsvPath);
			if (dsvFile is null)
			{
				_logger.LogError("Could not open the DSV file with path {ZipEntryPath}.", dsvPath);
				return null;
			}

			using (StreamReader reader = new StreamReader(dsvFile.Open()))
			{
				string dsv = reader.ReadToEnd();
				_logger.LogTrace("Successfully read the DSV file.");
				_logger.LogDebug(dsv);
				return dsv;
			}
		}
	}
}
