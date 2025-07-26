using DataspecNavigationBackend.Model;
using VDS.RDF;

namespace DataspecNavigationBackend.BusinessCoreLayer.Facade;

/// <summary>
/// A facade for the dotnetRdf library used to process the DSVs from Dataspecer.
/// </summary>
public interface IRdfProcessor
{
	IGraph CreateGraphFromRdfString(string rdfString);
	string WriteGraphToString(IGraph graph);
	IGraph ConvertDsvToOwl(IGraph dsvGraph); // Could consider adding more overloads but I will leave that for later.
}