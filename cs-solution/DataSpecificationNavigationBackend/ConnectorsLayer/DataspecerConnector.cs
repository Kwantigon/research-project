using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using System.IO.Compression;
using System.Net.Http;

namespace DataspecNavigationHelper.ConnectorsLayer;

public class DataspecerConnector(
	ILogger<DataspecerConnector> logger) : IDataspecerConnector
{
	private const string DATASPECER_JSONLD_ENDPOINT = "https://tool.dataspecer.com/api/preview/context.jsonld?iri=";
	private const string DATASPECER_DOWNLOAD_DOCUMENTATION_ENDPOINT = "https://tool.dataspecer.com/api/experimental/output.zip?iri=";

	private readonly HttpClient _httpClient = new HttpClient();
	private readonly ILogger<DataspecerConnector> _logger = logger;

	public async Task<string?> ExportPackageDocumentation(string packageIri)
	{
		string uri = DATASPECER_DOWNLOAD_DOCUMENTATION_ENDPOINT + packageIri;
		_logger.LogDebug("Downloading the Dataspecer package documentation from the URI: {URI}.", uri);
		byte[] data = await _httpClient.GetByteArrayAsync(uri);

		const string zipFilePath = "./documentation.zip";
		File.WriteAllBytes(zipFilePath, data);
		_logger.LogDebug("Package documentation stored in the local file system.");

		_logger.LogDebug("Opening the DSV for reading.");
		const string dsvPath = "en/dsv.ttl";
		using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Read))
		{
			ZipArchiveEntry? dsvFile = zip.GetEntry(dsvPath);
			if (dsvFile is null)
			{
				_logger.LogDebug("Could not open the DSV file with path {ZipEntryPath}.", dsvPath);
				return null;
			}

			using (StreamReader reader = new StreamReader(dsvFile.Open()))
			{
				string dsv = reader.ReadToEnd();
				_logger.LogDebug("Successfully read the DSV file.");
				_logger.LogDebug(dsv);
				return dsv;
			}
		}
	}

	/*private string ExportPackageDocumentationJsonLd(string packageIri)
	{
		Uri uri = new Uri(DATASPECER_JSONLD_ENDPOINT + packageIri);
		_logger.LogDebug("Gettings the package documentation from uri: {URI}", uri.ToString());
		Task<string> jsonLdTask = _httpClient.GetStringAsync(uri);
		jsonLdTask.Wait();
		return jsonLdTask.Result;
	}*/
}
