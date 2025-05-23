using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class DataSpecificationService(
	IDataspecerConnector dataspecerConnector) : IDataSpecificationService
{
	private readonly IDataspecerConnector _dataspecerConnector = dataspecerConnector;
	private readonly DsvToOwlConverter _dsvToOwlConverter = new DsvToOwlConverter();
	private readonly EntityFrameworkPlaceholder _database = new EntityFrameworkPlaceholder();

	public DataSpecification ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName)
	{
		string dsv = _dataspecerConnector.ExportDsvFromDataspecer();
		IGraph dsvGraph = new Graph();
		StringParser.Parse(dsvGraph, dsv);
		IGraph owlGraph = _dsvToOwlConverter.ConvertDsvGraphToOwlGraph(dsvGraph);
		IRdfWriter rdfWriter = new CompressingTurtleWriter();
		string owl = VDS.RDF.Writing.StringWriter.Write(owlGraph, rdfWriter);
		DataSpecification dataSpecification = new DataSpecification()
		{
			Name = (userGivenName != null ? userGivenName : "Todo: Give it the Dataspecer package name"),
			Iri = dataspecerPackageIri,
			Owl = owl
		};
		_database.Save(dataSpecification);
		return dataSpecification;
	}
}

internal class DsvToOwlConverter
{
	private const string DSV_CLASS_PROFILE = "https://w3id.org/dsv#ClassProfile";
	private const string DSV_OBJECT_PROPERTY_PROFILE = "https://w3id.org/dsv#ObjectPropertyProfile";
	private const string DSV_OBJECT_PROPERTY_RANGE = "https://w3id.org/dsv#objectPropertyRange";
	private const string DSV_DATATYPE_PROPERTY_PROFILE = "https://w3id.org/dsv#DatatypePropertyProfile";
	private const string DSV_DATATYPE_PROPERTY_RANGE = "https://w3id.org/dsv#datatypePropertyRange";
	private const string DSV_DOMAIN = "https://w3id.org/dsv#domain";
	private const string DSV_REUSES_PROPERTY_VALUE = "https://w3id.org/dsv#reusesPropertyValue";
	private const string DSV_REUSED_PROPERTY = "https://w3id.org/dsv#reusedProperty";
	private const string DSV_REUSED_FROM_RESOURCE = "https://w3id.org/dsv#reusedFromResource";
	private const string DSV_CARDINALITY = "https://w3id.org/dsv#cardinality";
	private const string CARDINALITY_1N = "https://w3id.org/dsv/cardinality#1n";
	private const string CARDINALITY_11 = "https://w3id.org/dsv/cardinality#11";
	private const string CARDINALITY_0N = "https://w3id.org/dsv/cardinality#0n";
	private const string CARDINALITY_01 = "https://w3id.org/dsv/cardinality#01";
	internal IGraph ConvertDsvGraphToOwlGraph(IGraph dsvGraph)
	{
		IGraph owlGraph = new Graph();
		owlGraph.NamespaceMap.Import(dsvGraph.NamespaceMap);

		foreach (Triple dsvTriple in dsvGraph.Triples)
		{
			INode subjectNode = dsvTriple.Subject;
			INode predicateNode = dsvTriple.Predicate;
			INode objectNode = dsvTriple.Object;

			// Rules to transform nodes to OWL
			if (objectNode.NodeType is NodeType.Uri)
			{
				string nodeUri = ((UriNode)objectNode).Uri.ToSafeString();

				if (nodeUri == DSV_CLASS_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdfs:Class")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:Class")));
				}

				if (nodeUri == DSV_OBJECT_PROPERTY_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:ObjectProperty")));
				}

				if (nodeUri == DSV_DATATYPE_PROPERTY_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:DatatypeProperty")));
				}
			}
			else if (objectNode.NodeType is NodeType.Blank)
			{
				string predicateUri = ((UriNode)predicateNode).Uri.ToSafeString();
				if (predicateUri == DSV_REUSES_PROPERTY_VALUE)
				{
					List<Triple> blankObjectNodeTriples = dsvGraph.GetTriplesWithSubject(objectNode).ToList();
					Triple? reusedProperty = blankObjectNodeTriples
						.Find(t => (t.Predicate.NodeType is NodeType.Uri) && ((UriNode)t.Predicate).Uri.ToSafeString() == DSV_REUSED_PROPERTY);
					Triple? reusedFromResource = blankObjectNodeTriples
						.Find(t => (t.Predicate.NodeType is NodeType.Uri) && ((UriNode)t.Predicate).Uri.ToSafeString() == DSV_REUSED_FROM_RESOURCE);

					if ((reusedProperty is not null) && (reusedFromResource is not null))
					{
						// Tady kod pro ziskani reused property jako napr. skos:prefLabel a skos:definition.
						Console.WriteLine("Tady kod pro ziskani reused property jako napr. skos:prefLabel a skos:definition.");
					}
				}
			}

			if (predicateNode.NodeType is NodeType.Uri)
			{
				string predicateUri = ((UriNode)predicateNode).Uri.ToSafeString();

				if (predicateUri == DSV_DOMAIN)
				{
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:domain"), objectNode));
				}

				if (predicateUri == DSV_DATATYPE_PROPERTY_RANGE || predicateUri == DSV_OBJECT_PROPERTY_RANGE)
				{
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:range"), objectNode));
				}

				if (predicateUri == DSV_CARDINALITY)
				{
					if (objectNode.NodeType is NodeType.Uri)
					{
						ILiteralNode literalNode;
						switch (((UriNode)objectNode).Uri.ToSafeString())
						{
							case CARDINALITY_11:
								literalNode = owlGraph.CreateLiteralNode("Cardinality of the property is one to one.", "en");
								break;
							case CARDINALITY_1N:
								literalNode = owlGraph.CreateLiteralNode("Cardinality of the property is one to many.", "en");
								break;
							case CARDINALITY_0N:
								literalNode = owlGraph.CreateLiteralNode("Cardinality of the property is zero to many.", "en");
								break;
							case CARDINALITY_01:
								literalNode = owlGraph.CreateLiteralNode("Cardinality of the property is zero to one.", "en");
								break;
							default:
								literalNode = owlGraph.CreateLiteralNode("Cardinality is not specified.", "en");
								break;
						}
						owlGraph.Assert(new Triple(subjectNode, owlGraph.CreateUriNode("rdfs:comment"), literalNode));
					}
				}
			}
		}

		return owlGraph;
	}
}