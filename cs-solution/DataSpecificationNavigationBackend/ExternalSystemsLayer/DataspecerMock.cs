namespace DataspecNavigationHelper.ExternalSystemsLayer;

public class DataspecerMock
{
	/// <summary>
	/// Return a JSON and pretend that it is the Dataspecer package representation.</br>
	/// I have to check the Dataspecer API to see how to get the packages.
	/// </summary>
	/// <param name="dataspecerPackageUri"></param>
	/// <returns></returns>
	public string GetDataspecerPackage(string dataspecerPackageUri)
	{
		return "{ package: \"mock package\" }";
	}
}
