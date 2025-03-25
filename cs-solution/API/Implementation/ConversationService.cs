using Backend.Abstractions;
using Backend.DTO;
using Backend.Model;
using Microsoft.VisualBasic;
using System.Text;

namespace Backend.Implementation;

public class ConversationService : IConversationService
{
	public UserMessage AddAndReturnUserMessageToConversation(Conversation conversation, string messageText, DateTime messageTimeStamp)
	{
		UserMessage userMessage = new UserMessage { Id = conversation.NextUnusedMessageId++, TextValue = messageText, TimeStamp = messageTimeStamp };
		conversation.Messages.Add(userMessage);
		return userMessage;
	}

	public void AddItemsToSubstructurePreview(Conversation conversation, IReadOnlyList<DataSpecificationItem> itemsToAdd)
	{
		conversation.NextQuestionSubstructurePreview = new DataSpecificationSubstructure();
	}

	public DataSpecificationSubstructure? ConfirmSubstructurePreview(Conversation conversation)
	{
		conversation.DataSpecificationSubstructure = conversation.NextQuestionSubstructurePreview;
		conversation.NextQuestionSubstructurePreview = null;
		return conversation.DataSpecificationSubstructure;
	}

	public DataSpecificationSubstructure? CreateDataSpecificationSubstructureForConversation(Conversation conversation, IReadOnlyList<DataSpecificationItem> dataSpecificationItems)
	{
		return new DataSpecificationSubstructure();
	}

	public Conversation CreateNewConversation(DataSpecification dataSpecification, string? conversationTitle = null)
	{
		Conversation conversation = new(dataSpecification, conversationTitle);
		conversation.InitializeConversation();
		return conversation;
	}

	public PositiveSystemAnswer GeneratePositiveSystemAnswer(string sparqlQuery, IReadOnlyList<DataSpecificationItem> mappedItems, Conversation conversation)
	{
		StringBuilder answerBuilder = new StringBuilder();
		answerBuilder.Append("The data you want can be retrieved using the following Sparql query: ");
		answerBuilder.Append(sparqlQuery);
		answerBuilder.AppendLine();
		answerBuilder.AppendLine();
		answerBuilder.Append("Some parts of your question can be expanded. The following list contains words, which I think can be expanded upon. You can click on each word for more information.");
		List<DataSpecificationItemDTO> dataSpecificationItemsDTO = [];
		foreach (var item in mappedItems)
		{
			answerBuilder.Append("- ");
			answerBuilder.AppendLine(item.Name);
			dataSpecificationItemsDTO.Add(
			new DataSpecificationItemDTO
			{
				Name = item.Name,
				Location = $"/data-specifications/{conversation.DataSpecification.Id}/items/{item.Id}"
			});
		}

		return new PositiveSystemAnswer()
		{
			Id = conversation.NextUnusedMessageId++,
			TimeStamp = DateTime.Now,
			TextValue = answerBuilder.ToString(),
			MatchedItems = mappedItems
		};
	}

	public void UpdateQuestionPreview(Conversation conversation, string questionText)
	{
		conversation.NextQuestionPreview = new UserPreviewMessage { TextValue = questionText, TimeStamp = DateTime.Now };
	}
}
