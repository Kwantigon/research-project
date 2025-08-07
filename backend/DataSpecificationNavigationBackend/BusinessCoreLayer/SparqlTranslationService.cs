using DataSpecificationNavigationBackend.Model;
using System.Text;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer;

public class SparqlTranslationService
{
	public string TranslateSubstructure(IReadOnlyCollection<DataSpecificationItem> substructure)
	{
		List<DataSpecificationItem> classes = substructure.Where(item => item.Type is ItemType.Class).ToList();
		List<DataSpecificationItem> objectProperties = substructure.Where(item => item.Type is ItemType.ObjectProperty).ToList();
		List<DataSpecificationItem> datatypeProperties = substructure.Where(item => item.Type is ItemType.DatatypeProperty).ToList();

		Dictionary<string, string> classesVarMap = classes.ToDictionary(
			c => c.Iri,
			c => c.Label.ToLower().Replace(' ', '_')
		);

		StringBuilder sparql = new();
		sparql.AppendLine("SELECT *");
		sparql.AppendLine("WHERE {");

		foreach (DataSpecificationItem classItem in classes)
		{
			sparql.AppendLine($"\t{classesVarMap[classItem.Iri]} a {classItem.Iri} .");
		}

		sparql.AppendLine(); // Leave 1 empty line between classes and properties.

		foreach (DataSpecificationItem property in objectProperties)
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

			sparql.AppendLine($"\t{domainVar} {property.Iri} {rangeVar} .");
		}

		sparql.AppendLine(); // Leave 1 empty line between object properties and datatype properties.

		foreach (DataSpecificationItem property in datatypeProperties)
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
			sparql.AppendLine($"\t{domainVar} {property.Iri} {rangeVar} .");

			// If filtering is supported, add it here.
			//sparql.AppendLine($"\t{BuildFilterClause(domainVar, DataType.String, rangeVar)}");
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
