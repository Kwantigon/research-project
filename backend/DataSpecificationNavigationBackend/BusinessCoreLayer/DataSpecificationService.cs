using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.BusinessCoreLayer.Facade;
using DataSpecificationNavigationBackend.ConnectorsLayer;
using DataSpecificationNavigationBackend.ConnectorsLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using VDS.RDF;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

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
		_logger.LogTrace("Converting the DSV to OWL.");
		DsvToOwlConverter converter = new DsvToOwlConverter();
		IGraph owlGraph = converter.ConvertDsvGraphToOwlGraph(dsvGraph, out List<DataSpecificationItem> extractedItems);
		string owl = _rdfProcessor.WriteGraphToString(owlGraph);

		_logger.LogDebug(owl);
		DataSpecification dataSpecification = new DataSpecification()
		{
			DataspecerPackageUuid = dataspecerPackageUuid,
			Name = dataspecerPackageName,
			Owl = owl,
		};
		await _database.DataSpecifications.AddAsync(dataSpecification);

		foreach (var item in extractedItems)
		{
			if (string.IsNullOrEmpty(item.Label))
			{
				// Failed to retrieve the label during DSV to OWL conversion.
				// Use the item's iri as label (but only the fragment part).
				string iriFragment = new Uri(item.Iri).Fragment.Substring(1); // The first character is '#' so I don't want that.
				iriFragment = Uri.UnescapeDataString(iriFragment);
				iriFragment.Replace('-', ' ');
				item.Label = iriFragment;
			}
			item.DataSpecificationId = dataSpecification.Id;
			item.DataSpecification = dataSpecification;
		}

		await _database.DataSpecificationItems.AddRangeAsync(extractedItems);
		await _database.SaveChangesAsync();
		return dataSpecification;
	}

	public async Task<DataSpecification?> GetDataSpecificationAsync(int dataSpecificationId)
	{
		return await _database.DataSpecifications.SingleOrDefaultAsync(dataSpec => dataSpec.Id == dataSpecificationId);
	}

	public async Task<DataSpecificationItem?> GetDataSpecificationItemAsync(int dataSpecificationId, string itemIri)
	{
		return await _database.DataSpecificationItems.SingleOrDefaultAsync(item => item.DataSpecificationId == dataSpecificationId && item.Iri == itemIri);
	}

	public async Task GenerateItemSummaryAsync(DataSpecificationItem item)
	{
		if (item.Summary != null)
		{
			_logger.LogInformation("Item summary already exists. It will not be generated again.");
			return;
		}

		_logger.LogTrace("Generating a summary for item [IRI={Iri}, Label={Label}].", item.Iri, item.Label);
		string summary = await _llmConnector.GenerateItemSummaryAsync(item);
		_logger.LogTrace("Summary generated successfully: {Summary}", summary);

		_logger.LogTrace("Setting the item.Summary property and saving to database.");
		item.Summary = summary;
		await _database.SaveChangesAsync();
	}

	public async Task<List<DataSpecificationItem>> GetItemsByIriListAsync(int dataSpecificationId, List<string> itemIriList)
	{
		return await _database.DataSpecificationItems
			.Where(item => item.DataSpecificationId == dataSpecificationId && itemIriList.Contains(item.Iri))
			.ToListAsync();
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

	internal IGraph ConvertDsvGraphToOwlGraph(IGraph dsvGraph, out List<DataSpecificationItem> extractedItems)
	{
		IGraph owlGraph = new Graph();
		owlGraph.NamespaceMap.Import(dsvGraph.NamespaceMap);

		Dictionary<string, DataSpecificationItem> itemsMap = new();
		foreach (Triple dsvTriple in dsvGraph.Triples)
		{
			INode subjectNode = dsvTriple.Subject;
			INode predicateNode = dsvTriple.Predicate;
			INode objectNode = dsvTriple.Object;

			// Rules to transform nodes to OWL
			if (objectNode.NodeType is NodeType.Uri)
			{
				string objectUri = ((UriNode)objectNode).Uri.ToSafeString();

				if (objectUri == DSV_CLASS_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdfs:Class")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:Class")));
					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					dsi.Type = ItemType.Class;
				}

				if (objectUri == DSV_OBJECT_PROPERTY_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:ObjectProperty")));
					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					dsi.Type = ItemType.ObjectProperty;
				}

				if (objectUri == DSV_DATATYPE_PROPERTY_PROFILE)
				{
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("rdf:Property")));
					owlGraph.Assert(new Triple(subjectNode, predicateNode, dsvGraph.CreateUriNode("owl:DatatypeProperty")));
					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					dsi.Type = ItemType.DatatypeProperty;
				}
			}

			if (predicateNode.NodeType is NodeType.Uri)
			{
				string predicateUri = ((UriNode)predicateNode).Uri.ToSafeString();

				if (predicateUri == DSV_DOMAIN)
				{
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:domain"), objectNode));

					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					dsi.DomainItemIri = ((UriNode)objectNode).ToString();
				}

				if (predicateUri == DSV_DATATYPE_PROPERTY_RANGE || predicateUri == DSV_OBJECT_PROPERTY_RANGE)
				{
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:range"), objectNode));

					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					dsi.RangeItemIri = ((UriNode)objectNode).ToString();
				}

				if (predicateUri == DSV_REUSES_PROPERTY_VALUE)
				{
					IEnumerable<Triple> reuseInfoTriples = dsvGraph.GetTriplesWithSubject(objectNode);
					INode? reusedPropertyNode = null; // usually it's either skos:prefLabel or skos:definition.
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

					if (reusedPropertyNode == null || reusedFromResourceNode == null)
					{
						// To do: log error.
						continue;
						//throw new Exception("reusedPropertyNode or reusedResourceNode was null.");
					}
					else if (reusedPropertyNode.NodeType != NodeType.Uri || reusedFromResourceNode.NodeType != NodeType.Uri)
					{
						// To do: log error.
						continue;
						throw new Exception("reusedPropertyNode or reusedResourceNode is not of type URI.");
					}

					IGraph reusedResourceGraph = new Graph();
					string reusedResourceUri = ((UriNode)reusedFromResourceNode).Uri.ToSafeString();
					if (reusedResourceUri.StartsWith("https://slovník.gov.cz"))
					{
						/*
						 * https://slovník.gov.cz endpoint does not return turtle.
						 * The turtle endpoint is https://xn--slovnk-7va.gov.cz/sparql?query=define%20sql%3Adescribe-mode%20%22CBD%22%20%20DESCRIBE%20%3C...........%3E&output=text%2Fturtle
						 */
						reusedResourceGraph.LoadFromUri(GetSlovnikGovRdfEndpointUri(reusedResourceUri));
					}
					else
					{
						reusedResourceGraph.LoadFromUri(new Uri(reusedResourceUri));
					}

					IUriNode? uriNodeToLookFor = reusedResourceGraph.GetUriNode(((UriNode)reusedPropertyNode).Uri);
					if (uriNodeToLookFor is not null)
					{
						Triple? reusedPropertyTriple = reusedResourceGraph.GetTriplesWithPredicate(uriNodeToLookFor).FirstOrDefault();
						if (reusedPropertyTriple is not null)
						{
							if (reusedPropertyTriple is not null)
							{
								if (uriNodeToLookFor.Uri.ToSafeString() == SKOS_PREF_LABEL)
								{
									string label = ((LiteralNode)reusedPropertyTriple.Object).Value;
									owlGraph.Assert(subjectNode, owlGraph.CreateUriNode("rdfs:label"), owlGraph.CreateLiteralNode(label));
									string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
									itemsMap.TryGetValue(subjectUri, out var dsi);
									if (dsi is null)
									{
										dsi = new DataSpecificationItem()
										{
											Iri = subjectUri
										};
										itemsMap[dsi.Iri] = dsi;
									}
									dsi.Label = label;
								}

								if (uriNodeToLookFor.Uri.ToSafeString() == SKOS_PREF_DEFINITION)
								{
									string definition = ((LiteralNode)reusedPropertyTriple.Object).Value;
									owlGraph.Assert(subjectNode, owlGraph.CreateUriNode("owl:AnnotationProperty"), owlGraph.CreateLiteralNode(definition));
								}
							}
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

		extractedItems = itemsMap.Select(pair => pair.Value).ToList();
		return owlGraph;
	}

	private Uri GetSlovnikGovRdfEndpointUri(string resourceUri)
	{
		//string escaped = Uri.EscapeDataString(resourceUri);
		const string SLOVNIK_GOV_URI_TEMPLATE = "https://xn--slovnk-7va.gov.cz/sparql?query=define%20sql%3Adescribe-mode%20%22CBD%22%20%20DESCRIBE%20%3C{0}%3E&output=text%2Fturtle";
		string uri = string.Format(SLOVNIK_GOV_URI_TEMPLATE, resourceUri);
		return new Uri(uri);
	}
}