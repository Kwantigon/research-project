﻿using DataSpecificationNavigatorBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigatorBackend.Model;
using System.Text;

namespace DataSpecificationNavigatorBackend.BusinessCoreLayer.SparqlTranslation;

public class SparqlTranslationService(
	ILogger<SparqlTranslationService> logger) : ISparqlTranslationService
{
	/*
	 * To do: This seems to work fine.
	 *				but I want to make the Sparql query a bit more readable
	 *				by having better variable names.
	 *				Will come back to this later.
	 */

	private readonly ILogger<SparqlTranslationService> _logger = logger;

	public string TranslateSubstructure(DataSpecificationSubstructure substructure)
	{
		if (substructure.ClassItems.Count == 0)
			return "SELECT * WHERE {}";
		var graph = FromDataSpecification(substructure);
		return GenerateSparqlQuery(graph);
	}

	private QueryGraph FromDataSpecification(DataSpecificationSubstructure substructure)
	{
		QueryGraph graph = new();
		Dictionary<string, QueryNode> nodeMap = [];

		// Create all nodes.
		foreach (DataSpecificationSubstructure.ClassItem classItem in substructure.ClassItems)
		{
			QueryNode node = graph.GetOrCreateNode(classItem.Iri, classItem.Label, classItem.IsSelectTarget);
			nodeMap[node.Iri] = node;
		}

		// Add edges to nodes (object properties and datatype properties).
		foreach (DataSpecificationSubstructure.ClassItem classItem in substructure.ClassItems)
		{
			QueryNode node = nodeMap[classItem.Iri];

			// Add object properties.
			foreach (DataSpecificationSubstructure.PropertyItem objectProperty in classItem.ObjectProperties)
			{
				if (!nodeMap.TryGetValue(objectProperty.Range, out QueryNode? targetNode))
				{
					_logger.LogError("The object property {PropertyLabe} does not have its QueryNode range in the nodeMap.", objectProperty.Label);
					throw new Exception("Failed to retrieve the QueryNode that is the object property's range.");
				}

				node.OutgoingEdges.Add(new QueryEdge
				{
					PropertyIri = objectProperty.Iri,
					PropertyLabel = objectProperty.Label,
					Target = targetNode,
					IsOptional = objectProperty.IsOptional
				});
			}

			// Add datatype properties.
			foreach (DataSpecificationSubstructure.PropertyItem datatypeProperty in classItem.DatatypeProperties)
			{
				node.DatatypeProperties.Add(new DatatypeNode
				{
					PropertyIri = datatypeProperty.Iri,
					PropertyLabel = datatypeProperty.Label,
					Range = datatypeProperty.Range,
					IsSelectTarget = true,   // For now, we treat all datatype properties as select targets.
					IsOptional = datatypeProperty.IsOptional
				});
			}
		}

		// Decide roots. Here we add all nodes without incoming edges
		// Doesn't handle cycles.
		var allTargets = new HashSet<QueryNode>(nodeMap.Values.SelectMany(n => n.OutgoingEdges.Select(e => e.Target)));
		foreach (var node in nodeMap.Values)
		{
			if (!allTargets.Contains(node))
			{
				graph.Roots.Add(node);
			}
		}
		// If no roots were found, add the first node as a root.
		// This happens in case of existing cycles.
		if (graph.Roots.Count == 0 && nodeMap.Count > 0)
		{
			graph.Roots.Add(nodeMap.Values.First());
		}

		return graph;
	}

	private string GenerateSparqlQuery(QueryGraph graph)
	{
		CreateVariableNames(graph);

		var sparql = new StringBuilder();
		var selectTargets = new HashSet<string>();

		sparql.Append("SELECT DISTINCT "); // Will add the SELECT targets later.
		sparql.AppendLine();

		sparql.AppendLine("WHERE {");
		var visited = new HashSet<QueryNode>();
		foreach (var root in graph.Roots)
		{
			GenerateNode(root, sparql, selectTargets, visited, root.VariableName!, 1);
		}
		sparql.AppendLine("}");

		// Add SELECT targets.
		sparql.Insert(7, string.Join(" ", selectTargets) + " ");
		// index == 7 because we're inserting after "SELECT "

		return sparql.ToString();
	}

	private void GenerateNode(QueryNode node, StringBuilder sparql, HashSet<string> selectTargets,
															HashSet<QueryNode> visited, string currentVar, int indentLevel)
	{
		if (visited.Contains(node))
			return;

		visited.Add(node);

		string indent = new string(' ', indentLevel * 2);

		// Triple for the type of the variable.
		sparql.AppendLine($"{indent}{currentVar} a <{node.Iri}> .");

		if (node.IsSelectTarget)
			selectTargets.Add(currentVar);

		// Datatype properties
		foreach (DatatypeNode dtProp in node.DatatypeProperties)
		{
			if (dtProp.IsOptional)
			{
				sparql.AppendLine($"{indent}OPTIONAL {{ {currentVar} <{dtProp.PropertyIri}> {dtProp.VariableName} . }}");
				selectTargets.Add(dtProp.VariableName!);
			}
			else
			{
				sparql.AppendLine($"{indent}{currentVar} <{dtProp.PropertyIri}> {dtProp.VariableName} .");
			}
		}

		// Object properties
		foreach (QueryEdge edge in node.OutgoingEdges)
		{
			if (edge.IsOptional)
			{
				sparql.AppendLine($"{indent}OPTIONAL {{");
				sparql.AppendLine($"{indent}  {currentVar} <{edge.PropertyIri}> {edge.Target.VariableName} .");
				GenerateNode(edge.Target, sparql, selectTargets, visited, edge.Target.VariableName!, indentLevel + 1);
				sparql.AppendLine($"{indent}}}");
			}
			else
			{
				sparql.AppendLine($"{indent}{currentVar} <{edge.PropertyIri}> {edge.Target.VariableName} .");
				GenerateNode(edge.Target, sparql, selectTargets, visited, edge.Target.VariableName!, indentLevel);
			}
		}

		visited.Remove(node); // Allow other branches to revisit
	}

	private void CreateVariableNames(QueryGraph graph)
	{
		// <string = variable, int = occurences>
		Dictionary<string, int> varNameOccurences = [];

		foreach (QueryNode node in graph.Nodes)
		{
			if (node.VariableName is null)
			{
				string variableName = node.Label.Replace(' ', '_');
				if (varNameOccurences.TryGetValue(variableName, out int count))
				{
					node.VariableName = $"?{variableName}{count}";
					count++;
					varNameOccurences[variableName] = count;
				}
				else
				{
					node.VariableName = $"?{variableName}0";
					varNameOccurences[variableName] = 1;
				}
			}

			foreach (DatatypeNode datatypeNode in node.DatatypeProperties)
			{
				if (datatypeNode.VariableName is null)
				{
					string variableName = datatypeNode.PropertyLabel.Replace(' ', '_');
					if (varNameOccurences.TryGetValue(variableName, out int count))
					{
						datatypeNode.VariableName = $"?{variableName}{count}";
						count++;
						varNameOccurences[variableName] = count;
					}
					else
					{
						datatypeNode.VariableName = $"?{variableName}0";
						varNameOccurences[variableName] = 1;
					}
				}
			}
		}
	}
}
