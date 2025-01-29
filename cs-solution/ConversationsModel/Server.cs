using System.Collections;

namespace ConversationsModel;

public class Server
{
	private Dictionary<uint, Conversation> Conversations { get; set; } = new Dictionary<uint, Conversation>();

	private Dictionary<uint, DataSpecification> DataSpecifications { get; set; } = new Dictionary<uint, DataSpecification>();

	/// <summary>
	/// Process the given data specification and save it.
	/// Use the name stored in the DataSpecer as the name of the created data specification.
	/// </summary>
	/// <param name="uriOfDataSpec"></param>
	/// <returns></returns>
	public uint Post_DataSpecification(string uriOfDataSpec)
	{
		string dataSpecName = "Name_taken_from_dataspecer";
		return Post_DataSpecification(uriOfDataSpec, dataSpecName);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="uriOfDataSpec"></param>
	/// <param name="nameOfDataSpec"></param>
	/// <returns></returns>
	public uint Post_DataSpecification(string uriOfDataSpec, string nameOfDataSpec)
	{
		Console.WriteLine("Processing the data specification"); // No implementation in this first version though.

		DataSpecification dataSpecification = new DataSpecification(nameOfDataSpec);
		DataSpecifications.Add(dataSpecification.Id, dataSpecification);
		return dataSpecification.Id;
	}

	/// <summary>
	/// Creates a new conversation with the given data specification.
	/// </summary>
	/// <param name="dataSpecificationId"></param>
	/// <returns>The ID of the newly created conversation.</returns>
	/// <exception cref="Exception">If the data specification was not found</exception>
	public uint Post_Conversations(uint dataSpecificationId)
	{
		if (!DataSpecifications.TryGetValue(dataSpecificationId, out DataSpecification? dataSpecification))
		{
			throw new Exception("No data specification with ID " + dataSpecificationId + "was found");
		}

		Conversation conversation = new Conversation(dataSpecification);
		Conversations.Add(conversation.Id, conversation);
		return conversation.Id;
	}
}
