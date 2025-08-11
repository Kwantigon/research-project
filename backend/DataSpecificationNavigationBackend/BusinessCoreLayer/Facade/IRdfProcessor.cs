using DataSpecificationNavigationBackend.Model;
using VDS.RDF;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Facade;

/// <summary>
/// A facade for the dotnetRdf library used to process the DSVs from Dataspecer.
/// </summary>
public interface IRdfProcessor
{
	IGraph CreateGraphFromRdfString(string rdfString);
	string WriteGraphToString(IGraph graph);
	IGraph ConvertDsvGraphToOwlGraph(IGraph dsvGraph, out List<DataSpecificationItem> extractedItems);
}