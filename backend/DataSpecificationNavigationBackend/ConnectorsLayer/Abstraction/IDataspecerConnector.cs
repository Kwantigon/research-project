namespace DataspecNavigationBackend.ConnectorsLayer.Abstraction;

public interface IDataspecerConnector
{
	/// <summary>
	/// Retrieve the DSV documentation of the Dataspecer package.
	/// </summary>
	/// <param name="packageIri">UUID of the package.</param>
	/// <returns>
	/// The retrieved DSV in a string.</br>
	/// <see langword="null"/> if the DSV retrieval failed.
	/// </returns>
	Task<string?> ExportPackageDocumentation(string packageIri);
}
