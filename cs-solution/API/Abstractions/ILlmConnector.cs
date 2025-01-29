using BackendApi.Model;

namespace BackendApi.Abstractions;

public interface ILlmConnector
{
	/// <summary>
	/// Sends a prompt containing a query in natural language to be translated to Sparql.
	/// </summary>
	/// <param name="userQuery">The query from the user in natural language.</param>
	/// <returns>A Sparql query that corresponds to the natural language query.</returns>
	string TranslateToSparql(string userQuery);

	/// <summary>
	/// Sends a prompt asking for properties, which can be used to expand the query in natural language.
	/// </summary>
	/// <param name="propertyName">The property, for which expansion properties are required.</param>
	/// <returns>A JSON containing the properties, which can be used to expand the query.</returns>
	string GetExpansionProperties(string propertyName);

	/// <summary>
	/// Sends a prompt asking for a summary of one concrete property.
	/// </summary>
	/// <param name="propertyId">The name of the property to be summarized.</param>
	/// <returns>A JSON containing the summary of the property.</returns>
	string GetPropertySummary(DataSpecification dataSpecification, uint propertyId);
}
