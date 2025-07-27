using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.BusinessCoreLayer.Abstraction;
using DataspecNavigationBackend.BusinessCoreLayer.Facade;
using DataspecNavigationBackend.ConnectorsLayer;
using DataspecNavigationBackend.ConnectorsLayer.Abstraction;
using DataspecNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using VDS.RDF;

namespace DataspecNavigationBackend.BusinessCoreLayer;

public class DataSpecificationService(
	ILogger<DataSpecificationService> logger,
	IDataspecerConnector dataspecerConnector,
	IRdfProcessor rdfProcessor,
	AppDbContext appDbContext,
	ILlmConnector llmConnector) : IDataSpecificationService
{
	private readonly ILogger<DataSpecificationService> _logger = logger;
	private readonly IDataspecerConnector _dataspecerConnector = dataspecerConnector;
	private readonly IRdfProcessor _rdfProcessor = rdfProcessor;
	private readonly AppDbContext _database = appDbContext;
	private readonly ILlmConnector _llmConnector = llmConnector;

	public async Task<DataSpecification?> ExportDataSpecificationFromDataspecerAsync(string dataspecerPackageUuid, string dataspecerPackageName)
	{
		string? dsv = await _dataspecerConnector.ExportPackageDocumentation(dataspecerPackageUuid);
		if (string.IsNullOrEmpty(dsv))
		{
			_logger.LogError("The exported package DSV is either null or empty.");
			return null;
		}
		_logger.LogDebug("Exported DSV:\n{Content}", dsv);

		IGraph dsvGraph = _rdfProcessor.CreateGraphFromRdfString(dsv);
		_logger.LogDebug("Converting the DSV to OWL.");
		IGraph owlGraph = ConvertDsvToOwl(dsvGraph);
		string owl = _rdfProcessor.WriteGraphToString(owlGraph);

		_logger.LogDebug(owl);
		DataSpecification dataSpecification = new DataSpecification()
		{
			DataspecerPackageUuid = dataspecerPackageUuid,
			Name = dataspecerPackageName,
			Owl = owl,
		};

		await _database.DataSpecifications.AddAsync(dataSpecification);
		await _database.SaveChangesAsync();
		return dataSpecification;

		// Mock
		/*DataSpecification dataSpecification = new DataSpecification()
		{
			DataspecerPackageUuid = dataspecerPackageUuid,
			Name = dataspecerPackageName,
			Owl = File.ReadAllText("C:/Users/nquoc/MatFyz/research-project/repo/research-project/backend/DataSpecificationNavigationBackend/local/insurance.owl.ttl"),
		};

		await _database.DataSpecifications.AddAsync(dataSpecification);
		await _database.SaveChangesAsync();
		return dataSpecification;*/
	}

	public async Task<DataSpecification?> GetDataSpecificationAsync(int dataSpecificationId)
	{
		return await _database.DataSpecifications.SingleOrDefaultAsync(dataSpec => dataSpec.Id == dataSpecificationId);
	}

	public async Task<IResult> GetItemSummaryAsync(int dataSpecificationId, string itemIri)
	{
		throw new NotImplementedException();
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