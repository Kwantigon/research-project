using DataspecNavigationHelper.ConnectorsLayer.Abstraction;

namespace DataspecNavigationHelper.ConnectorsLayer;

public class DataspecerConnectorMock : IDataspecerConnector
{
	public string ExportDsvFromDataspecer()
	{
		return File.ReadAllText(Path.Combine("Resources", "obce.dsv.ttl"));
	}
}
