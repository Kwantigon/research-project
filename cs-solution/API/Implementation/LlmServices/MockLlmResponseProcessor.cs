using Backend.Abstractions.LlmServices;
using Backend.Model;

namespace Backend.Implementation.LlmServices
{
	public class MockLlmResponseProcessor : ILlmResponseProcessor
	{
		public List<DataSpecificationItem> ProcessItemsMappingResponse(string llmResponse)
		{
			
			return new List<DataSpecificationItem>
			{
				new DataSpecificationItem
				{
					Id = 0,
					Name = "Tournament"
				}
			};
		}

		public DataSpecificationItemSummary ProcessItemsSummaryResponse(string llmResponse)
		{
			const string mockSummaryString =
				"""
				Badminton tournaments are organized by the Badminton World Federation (BWF). Tournaments are divided into grades and each grade is further divided into multiple levels. Different tournaments take place in different countries. Most tournaments are annual but some have longer repetition intervals.
				Some properties related to badminton tournament are:
				- Grade.
				- Repetition interval.
				- Sponsoring brand.
				- Participants.
				""";
			return new DataSpecificationItemSummary
			{
				TextualSummary = mockSummaryString,
				RelatedItemsIds = new List<uint> { 1, 2, 3 }
			};
		}

		public string ProcessQuestionPreviewResponse(string llmResponse)
		{
			//return "I would like to see all tournaments, their sponsors, the players participating and their nationalities, which have taken place this year.";
			return "I would like to see all tournaments that which have taken place this year and the tournament sponsors and the participating players.";
		}
	}
}
