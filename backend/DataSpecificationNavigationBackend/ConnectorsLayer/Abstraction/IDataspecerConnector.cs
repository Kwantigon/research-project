namespace DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;

public interface IDataspecerConnector
{
	Task<string?> ExportDsvFileFromPackage(string packageIri);
	Task<string?> ExportOwlFileFromPackage(string packageIri);
}
