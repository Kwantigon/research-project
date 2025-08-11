using DataSpecificationNavigationBackend.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.Facade;

public class RdfProcessor(
	ILogger<RdfProcessor> logger) : IRdfProcessor
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

	private readonly ILogger<RdfProcessor> _logger = logger;

	public IGraph CreateGraphFromRdfString(string rdfString)
	{
		IGraph graph = new Graph();
		StringParser.Parse(graph, rdfString);
		return graph;
	}

	public string WriteGraphToString(IGraph graph)
	{
		IRdfWriter rdfWriter = new CompressingTurtleWriter();
		string rdfTurtle = VDS.RDF.Writing.StringWriter.Write(graph, rdfWriter);
		return rdfTurtle;
	}

	public IGraph ConvertDsvGraphToOwlGraph(IGraph dsvGraph, out List<DataSpecificationItem> extractedItems)
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
					subjectUri = Uri.UnescapeDataString(subjectUri);
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
					subjectUri = Uri.UnescapeDataString(subjectUri);
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
					subjectUri = Uri.UnescapeDataString(subjectUri);
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
					subjectUri = Uri.UnescapeDataString(subjectUri);
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					string domainIri = ((UriNode)objectNode).ToString();
					dsi.DomainItemIri = Uri.UnescapeDataString(domainIri);
				}

				if (predicateUri == DSV_DATATYPE_PROPERTY_RANGE || predicateUri == DSV_OBJECT_PROPERTY_RANGE)
				{
					owlGraph.Assert(new Triple(subjectNode, dsvGraph.CreateUriNode("rdfs:range"), objectNode));

					string subjectUri = ((UriNode)subjectNode).Uri.ToSafeString();
					subjectUri = Uri.UnescapeDataString(subjectUri);
					itemsMap.TryGetValue(subjectUri, out var dsi);
					if (dsi is null)
					{
						dsi = new DataSpecificationItem()
						{
							Iri = subjectUri
						};
						itemsMap[dsi.Iri] = dsi;
					}
					string rangeIri = ((UriNode)objectNode).ToString();
					dsi.RangeItemIri = Uri.UnescapeDataString(rangeIri);
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
						_logger.LogError("Either `reusedPropertyNode` or `reusedFromResourceNode` is null.");
						continue;
					}
					else if (reusedPropertyNode.NodeType != NodeType.Uri || reusedFromResourceNode.NodeType != NodeType.Uri)
					{
						_logger.LogError("Either reusedPropertyNode or reusedFromResourceNode is not of NodeType.Uri");
						continue;
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
									subjectUri = Uri.UnescapeDataString(subjectUri);
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
		// reusedResourceUri is already escaped because the UriNode returns and escaped string.
		// string escaped = Uri.EscapeDataString(resourceUri);
		const string SLOVNIK_GOV_URI_TEMPLATE = "https://xn--slovnk-7va.gov.cz/sparql?query=define%20sql%3Adescribe-mode%20%22CBD%22%20%20DESCRIBE%20%3C{0}%3E&output=text%2Fturtle";
		string uri = string.Format(SLOVNIK_GOV_URI_TEMPLATE, resourceUri);
		return new Uri(uri);
	}
}
