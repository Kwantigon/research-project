using DataspecNavigationHelper.Model;
using VDS.RDF;

namespace DataspecNavigationHelper.BusinessCoreLayer.Facade;

/// <summary>
/// A facade for the dotnetRdf library used to process the DSVs from Dataspecer.
/// </summary>
public interface IRdfProcessor
{
	IGraph CreateGraphFromRdfString(string rdfString);
	string WriteGraphToString(IGraph graph);
	IGraph ConvertDsvToOwl(IGraph dsvGraph); // Could consider adding more overloads but I will leave that for later.
	static IRdfGraphBuilder GetGraphBuilder() => new DefaultGraphBuilder();

	public interface IRdfGraphBuilder
	{
		public IGraph Build();

		public IRdfGraphBuilder ImportNamespaceFrom(IGraph graph);

		public IRdfGraphBuilder AddTriple(INode subjectNode, INode predicateNode, INode objectNode);
	}
}

internal class DefaultGraphBuilder : IRdfProcessor.IRdfGraphBuilder
{
	private readonly IGraph _graph = new Graph();

	public IGraph Build() => _graph;

	public IRdfProcessor.IRdfGraphBuilder ImportNamespaceFrom(IGraph graph)
	{
		_graph.NamespaceMap.Import(graph.NamespaceMap);
		return this;
	}

	public IRdfProcessor.IRdfGraphBuilder AddTriple(INode subjectNode, INode predicateNode, INode objectNode)
	{
		_graph.Assert(new Triple(subjectNode, predicateNode, objectNode));
		return this;
	}
}