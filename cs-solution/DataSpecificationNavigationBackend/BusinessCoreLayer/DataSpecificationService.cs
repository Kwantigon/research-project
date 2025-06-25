using DataspecNavigationHelper.BusinessCoreLayer.Abstraction;
using DataspecNavigationHelper.BusinessCoreLayer.Facade;
using DataspecNavigationHelper.ConnectorsLayer;
using DataspecNavigationHelper.ConnectorsLayer.Abstraction;
using DataspecNavigationHelper.Model;
using VDS.RDF;

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

	public async Task<DataSpecification> ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName)
	{
		string? dsv = await _dataspecerConnector.ExportPackageDocumentation(dataspecerPackageIri);
		if (string.IsNullOrEmpty(dsv))
		{
			_logger.LogError("The exported package DSV is either null or empty.");
			throw new Exception("Exported DSV is null or empty.");
		}
		_logger.LogDebug("Exported DSV:\n{Content}", dsv);

		IGraph dsvGraph = _rdfProcessor.CreateGraphFromRdfString(dsv);
		_logger.LogDebug("Converting the DSV to OWL.");
		IGraph owlGraph = ConvertDsvToOwl(dsvGraph);
		string owl = _rdfProcessor.WriteGraphToString(owlGraph);

		_logger.LogDebug(owl);
		DataSpecification dataSpecification = new DataSpecification()
		{
			Name = (userGivenName != null ? userGivenName : "Todo: Give it the Dataspecer package name"),
			Iri = dataspecerPackageIri,
			Owl = owl
		};

		_database.Save(dataSpecification);
		return dataSpecification;
	}

	/*
	 * Tuto metodu použiju, pokud z Dataspeceru exportuju DSVčko.
	 * Momentálně ale exportuju json-ld a nijak to nezprácovávám, takže tuto metodu zakomentuju
	 * a použiju možná někdy později.
	 */
	/*public DataSpecification ExportDataSpecificationFromDataspecer(string dataspecerPackageIri, string? userGivenName)
	{
		string dsv = _dataspecerConnector.ExportPackageDocumentation(dataspecerPackageIri);
		IGraph dsvGraph = _rdfProcessor.CreateGraphFromRdfString(dsv);
		IGraph owlGraph = ConvertDsvToOwl(dsvGraph);
		string owl = _rdfProcessor.WriteGraphToString(owlGraph);
		DataSpecification dataSpecification = new DataSpecification()
		{
			Name = (userGivenName != null ? userGivenName : "Todo: Give it the Dataspecer package name"),
			Iri = dataspecerPackageIri,
			Owl = owl
		};
		_database.Save(dataSpecification);
		return dataSpecification;
	}*/

	public DataSpecification? GetDataSpecificationByIri(string dataSpecificationIri)
	{
		return _database.FindDataSpecificationByIri(dataSpecificationIri);
	}

	private IGraph ConvertDsvToOwl(IGraph dsvGraph)
	{
		DsvToOwlConverter converter = new DsvToOwlConverter();
		return converter.ConvertDsvGraphToOwlGraph(dsvGraph);
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
	private const string SKOS_PREF_LABEL = "http://www.w3.org/2004/02/skos/core#prefLabel";
	private const string SKOS_PREF_DEFINITION = "http://www.w3.org/2004/02/skos/core#definition";
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

				if (predicateUri == DSV_REUSES_PROPERTY_VALUE)
				{
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