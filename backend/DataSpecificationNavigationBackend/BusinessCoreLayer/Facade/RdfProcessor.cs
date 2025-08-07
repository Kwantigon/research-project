using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Facade;

public class RdfProcessor : IRdfProcessor
{
	public IGraph CreateGraphFromRdfString(string rdfString)
	{
		IGraph graph = new Graph();
		StringParser.Parse(graph, rdfString);
		return graph;
	}

	/// <summary>
	/// Write the graph to a string in RDF Turtle format.
	/// </summary>
	/// <param name="graph">The graph to write.</param>
	/// <returns>The string containing RDF Turtle.</returns>
	public string WriteGraphToString(IGraph graph)
	{
		IRdfWriter rdfWriter = new CompressingTurtleWriter(TurtleSyntax.W3C);
		string rdfTurtle = VDS.RDF.Writing.StringWriter.Write(graph, rdfWriter);
		return rdfTurtle;
	}

	public IGraph ConvertDsvToOwl(IGraph dsvGraph)
	{
		throw new NotImplementedException();
	}
}
