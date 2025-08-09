using DataSpecificationNavigationBackend.BusinessCoreLayer.Abstraction;
using DataSpecificationNavigationBackend.Model;
using System.Text;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

public class SparqlTranslationService(
	ILogger<SparqlTranslationService> logger) : ISparqlTranslationService
{
	private readonly ILogger<SparqlTranslationService> _logger = logger;

	public string TranslateSubstructure(DataSpecificationSubstructure substructure)
	{
		if (substructure.ClassItems.Count == 0)
		{
			_logger.LogError("The data specification substructure is empty.");
			return "SELECT * WHERE {}";
		}

		List<string> targetClassesIri = new();
		List<string> nonTargetClassesIri = new();
		Dictionary<string, string> classesVarMap = new();
		foreach (var classItem in substructure.ClassItems)
		{
			if (classItem.IsSelectTarget)
				targetClassesIri.Add(classItem.Iri);
			else
				nonTargetClassesIri.Add(classItem.Iri);

			classesVarMap.Add(classItem.Iri, classItem.Label.ToLower().Replace(' ', '_'));
		}


		StringBuilder sparql = new();
		sparql.Append("SELECT");
		if (targetClassesIri.Count == 0)
		{
			sparql.AppendLine(" *");
		}
		else
		{
			foreach (string targetIri in targetClassesIri)
			{
				sparql.Append($" ?{classesVarMap[targetIri]}");
			}
			sparql.AppendLine();
		}
		sparql.AppendLine("WHERE {");

		foreach (DataSpecificationSubstructure.ClassItem classItem in substructure.ClassItems)
		{
			sparql.AppendLine($"\t?{classesVarMap[classItem.Iri]} a {classItem.Iri} .");

			foreach (DataSpecificationSubstructure.PropertyItem property in classItem.ObjectProperties)
			{
				if (property.Domain is null || property.Range is null)
				{
					Console.WriteLine("ERROR: {0} {1} {2} .", property.Domain, property.Iri, property.Range);
					throw new Exception($"Domain or range of the property \"{property.Label}\" is null.");
				}

				string? domainVar = classesVarMap[property.Domain];
				string? rangeVar = classesVarMap[property.Range];
				if (domainVar is null || rangeVar is null)
				{
					Console.WriteLine("ERROR: domainVar = {0}, rangeVar = {1}.", domainVar, rangeVar);
					throw new Exception($"Domain variable or range variable of property \"{property.Label}\" is null.");
				}

				sparql.AppendLine($"\t?{domainVar} {property.Iri} ?{rangeVar} .");
			}

			foreach (DataSpecificationSubstructure.PropertyItem property in classItem.DatatypeProperties)
			{
				if (property.Domain is null || property.Range is null)
				{
					Console.WriteLine("ERROR: {0} {1} {2} .", property.Domain, property.Iri, property.Range);
					throw new Exception($"Domain or range of the property \"{property.Label}\" is null.");
				}

				string? domainVar = classesVarMap[property.Domain];
				if (domainVar is null)
				{
					throw new Exception($"Domain variable of property \"{property.Label}\" is null.");
				}

				string rangeVar = $"?{property.Label.ToLower().Replace(' ', '_')}";
				sparql.AppendLine($"\t?{domainVar} {property.Iri} {rangeVar} .");

				// If filtering is supported, add it here.
				//sparql.AppendLine($"\t{BuildFilterClause(domainVar, DataType.String, rangeVar)}");
			}

			sparql.AppendLine(); // Leave 1 empty line after each class item.
		}

		sparql.AppendLine("}");
		return sparql.ToString();
	}

	#region Not yet supported.
	// Just an example of how filtering could be implemented.
	private string BuildFilterClause(string domainVar, DataType datatype, string dataValue, ComparisonType comparisonType = ComparisonType.Equality)
	{
		// Currently not supported, but it the implementation could be something like:
		switch (datatype)
		{
			case DataType.Number:
				if (int.TryParse(dataValue, out int number))
				{
					return $"FILTER({domainVar} {ComparisonSign(comparisonType)} {number})";
				}
				else
				{
					return string.Empty;
				}
			case DataType.Date:
				return string.Empty; // Not implemented.
			default:
				return $"FILTER({domainVar} = \"{dataValue})\""; // Treat everything as a string by default.
		}
	}

	private enum DataType
	{
		String,
		Number,
		Date
	}
	private enum ComparisonType
	{
		Equality,
		GreaterThan,
		GreaterOrEqual,
		LessThan,
		LessOrEqual
	}

	private string ComparisonSign(ComparisonType comparisonType)
	{
		return comparisonType switch
		{
			ComparisonType.Equality => "=",
			ComparisonType.GreaterThan => ">",
			ComparisonType.GreaterOrEqual => ">=",
			ComparisonType.LessThan => "<",
			ComparisonType.LessOrEqual => "<=",
			_ => throw new NotImplementedException("Unknown comparison type: " + comparisonType.ToString()),
		};
	}
	#endregion Not yet supported.
}
