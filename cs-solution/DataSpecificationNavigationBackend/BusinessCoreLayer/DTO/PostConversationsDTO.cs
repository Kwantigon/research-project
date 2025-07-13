namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

/// <summary>
/// DTO for the POST /conversations endpoint.
/// </summary>
/// <param name="ConversationTitle">Title of the new conversation.</param>
/// <param name="DataSpecificationIri">IRI of the Dataspecer package used for the conversation.</param>
/// <param name="DataSpecificationName">Name under which to store the data specification.</param>
public record PostConversationsDTO(string ConversationTitle, string DataSpecificationIri, string DataSpecificationName);
