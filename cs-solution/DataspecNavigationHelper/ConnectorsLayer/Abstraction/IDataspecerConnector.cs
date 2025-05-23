namespace DataspecNavigationHelper.ConnectorsLayer.Abstraction;

public interface IDataspecerConnector
{
	// Tady si musim zjistit, co presne vraci Dataspecer endpoint a v jakem tvaru.

	/// <summary>
	/// For now, let's just say that Dataspecer returns the DSV in a looooong string.
	/// </summary>
	/// <returns></returns>
	string ExportDsvFromDataspecer();
}
