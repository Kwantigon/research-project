using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.Facade;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace DataspecNavigationHelper.BusinessCoreLayer;

public class DataSpecificationService(
	ILogger<DataSpecificationService> logger,
	IDataspecerConnector dataspecerConnector,
	IRdfProcessor rdfProcessor,
	EntityFrameworkPlaceholder entityFrameworkPlaceholder) : IDataSpecificationService
{
	private readonly ILogger<DataSpecificationService> _logger = logger;
	private readonly IDataspecerConnector _dataspecerConnector = dataspecerConnector;
	private readonly IRdfProcessor _rdfProcessor = rdfProcessor;
	private readonly EntityFrameworkPlaceholder _database = entityFrameworkPlaceholder;

	public DataSpecification ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName)
	{
		string dsv = _dataspecerConnector.ExportDsvFromDataspecer();
		IGraph dsvGraph = _rdfProcessor.CreateGraphFromRdfString(dsv);
		IGraph owlGraph = _rdfProcessor.ConvertDsvToOwl(dsvGraph);
		string owl = _rdfProcessor.WriteGraphToString(owlGraph);
		_rdfProcessor.SaveGraphToFile(owlGraph, "data-specification.owl.ttl");
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

	private readonly ILogger<DsvToOwlConverter> _logger = LoggerFactory
		.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
		.CreateLogger<DsvToOwlConverter>();

	internal IGraph ConvertDsvGraphToOwlGraph(IGraph dsvGraph)
	{
		IGraph owlGraph = new Graph();
		owlGraph.NamespaceMap.Import(dsvGraph.NamespaceMap);
		_logger.LogDebug("Imported namespace map from the dsvGraph to owlGraph.");

		foreach (Triple dsvTriple in dsvGraph.Triples)
		{
			_logger.LogDebug("===== Processing a new triple.");
			INode subjectNode = dsvTriple.Subject;
			INode predicateNode = dsvTriple.Predicate;
			INode objectNode = dsvTriple.Object;

			// Rules to transform nodes to OWL
			if (objectNode.NodeType is NodeType.Uri)
			{
				_logger.LogDebug("Object node is an UriNode.");
				string nodeUri = ((UriNode)objectNode).Uri.ToSafeString();

				if (nodeUri == DSV_CLASS_PROFILE)
				{
					_logger.LogDebug("Object node is DSV_CLASS_PROFILE.");
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdfs:Class")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:Class")));
					_logger.LogDebug("Added rdfs:Class and owl:Class to the owlGraph.");
				}

				if (nodeUri == DSV_OBJECT_PROPERTY_PROFILE)
				{
					_logger.LogDebug("Object node is DSV_OBJECT_PROPERTY_PROFILE.");
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:ObjectProperty")));
					_logger.LogDebug("Added rdf:Property and owl:ObjectProperty to the owlGraph.");
				}

				if (nodeUri == DSV_DATATYPE_PROPERTY_PROFILE)
				{
					_logger.LogDebug("Object node is DSV_DATATYPE_PROPERTY_PROFILE.");
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:DatatypeProperty")));
					_logger.LogDebug("Added rdf:Property and owl:DatatypeProperty to the owlGraph.");
				}
			}

			if (predicateNode.NodeType is NodeType.Uri)
			{
				_logger.LogDebug("Predicate node is an UriNode.");
				string predicateUri = ((UriNode)predicateNode).Uri.ToSafeString();

				if (predicateUri == DSV_DOMAIN)
				{
					_logger.LogDebug("Predicate node is DSV_DOMAIN.");
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:domain"), objectNode));
					_logger.LogDebug("Added rdfs:domain to the owlGraph.");
				}

				if (predicateUri == DSV_DATATYPE_PROPERTY_RANGE || predicateUri == DSV_OBJECT_PROPERTY_RANGE)
				{
					_logger.LogDebug("Predicate node is DSV_DATATYPE_PROPERTY_RANGE or DSV_OBJECT_PROPERTY_RANGE.");
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:range"), objectNode));
					_logger.LogDebug("Added rdfs:range to the owlGraph.");
				}

				if (predicateUri == DSV_REUSES_PROPERTY_VALUE)
				{
					_logger.LogDebug("Predicate node is DSV_REUSES_PROPERTY_VALUE.");
					_logger.LogDebug("Getting triples with the object node as the subject of the triple.");
					IEnumerable<Triple> reuseInfoTriples = dsvGraph.GetTriplesWithSubject(objectNode);
					INode? reusedPropertyNode = null;
					INode? reusedFromResourceNode = null;
					foreach (Triple reuseTriple in reuseInfoTriples)
					{
						if (reuseTriple.Predicate.NodeType == NodeType.Uri)
						{
							string uri = ((UriNode)reuseTriple.Predicate).Uri.ToSafeString();
							if (uri == DSV_REUSED_PROPERTY)
							{
								reusedPropertyNode = reuseTriple.Object;
							}
							if (uri == DSV_REUSED_FROM_RESOURCE)
							{
								reusedFromResourceNode = reuseTriple.Object;
							}
						}
					}

					// For now, throw an exception.
					// But I will likely move this to a separate method.
					// Then do not throw exception - assume that the reused property cannot be retrieved
					// and continue onto the next triple in the graph.
					if (reusedPropertyNode == null || reusedFromResourceNode == null)
					{
						throw new Exception("reusedPropertyNode or reusedResourceNode was null.");
					}
					else if (reusedPropertyNode.NodeType != NodeType.Uri || reusedFromResourceNode.NodeType != NodeType.Uri)
					{
						throw new Exception("reusedPropertyNode or reusedResourceNode is not of type URI.");
					}
					_logger.LogDebug("The predicate to look for: " + ((UriNode)reusedPropertyNode).Uri);
					_logger.LogDebug("The URI where to look for the predicate: " + ((UriNode)reusedFromResourceNode).Uri);

					IGraph reusedResourceGraph = new Graph();
					reusedResourceGraph.LoadFromUri(((UriNode)reusedFromResourceNode).Uri);
					IUriNode? uriNodeToLookFor = reusedResourceGraph.GetUriNode(((UriNode)reusedPropertyNode).Uri);
					if (uriNodeToLookFor is not null)
					{
						Triple? reusedPropertyTriple = reusedResourceGraph.GetTriplesWithPredicate(uriNodeToLookFor).FirstOrDefault();
						if (reusedPropertyTriple is not null)
						{
							// tady zpracovani podle toho, jestli to je skos:prefLabel nebo skos:definition.
						}
					}

				}

				if (predicateUri == DSV_CARDINALITY)
				{
					_logger.LogDebug("Predicate node is DSV_CARDINALITY.");
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
						_logger.LogDebug("Added rdfs:comment with the property cardinality to the owlGraph.");
					}
				}
			}
			_logger.LogDebug("==============================");
		}

		return owlGraph;
	}
}