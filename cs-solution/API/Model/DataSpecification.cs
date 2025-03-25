using Backend.DTO;

namespace Backend.Model;

public class DataSpecification
{
	private static uint _nextUnusedId = 0;

	public DataSpecification(string? dataSpecificationName, string dataspecerIri)
	{
		Id = _nextUnusedId++;
		Name = dataSpecificationName != null ? dataSpecificationName : $"Unnamed data specification #{Id}";
		DataspecerIri = dataspecerIri;
	}

	public uint Id { get; }

	public string Name { get; set; }

	public string DataspecerIri { get; set; }

	public List<DataSpecificationItem> Items { get; set; } = new() // Mock items.
	{
		new DataSpecificationItem
		{
			Id = 0,
			Name = "Tournament",
			Summary = new DataSpecificationItemSummary
			{
				RelatedItemsIds = new List<uint> { 1, 2, 3 },
				TextualSummary =
					"""
					Badminton tournaments are organized by the Badminton World Federation (BWF). Tournaments are divided into grades and each grade is further divided into multiple levels. Different tournaments take place in different countries. Most tournaments are annual but some have longer repetition intervals.
					Some properties related to badminton tournament are:
					- Grade.
					- Repetition interval.
					- Sponsoring brand.
					- Participants.
					"""
			}
		},
		new DataSpecificationItem
		{
			Id = 1,
			Name = "Tournament grade",
			Summary = new DataSpecificationItemSummary
			{
				RelatedItemsIds = new List<uint> { },
				TextualSummary =
					"""
					The grade of tournament determines how prestigious the tournament is.
					Some properties related to grade are: there are none.
					"""
			}
		},
		new DataSpecificationItem
		{
			Id = 2,
			Name = "Sponsoring brand",
			Summary = new DataSpecificationItemSummary
			{
				RelatedItemsIds = new List<uint> { 4, 5, 6, 7 },
				TextualSummary =
					"""
					Big companies that make and sell badminton equipment will often sponsor tournaments and players. A lot of tournaments even have the company's name in the tournament's name (for example Yonex All England Open).
					Some properties related to brand are:
					- Name.
					- Founding date.
					- Founder.
					- Sponsored players.
					"""
			}
		},
		new DataSpecificationItem
		{
			Id = 3,
			Name = "Participants",
			Summary = new DataSpecificationItemSummary
			{
				RelatedItemsIds = new List<uint> { 8, 9, 10, 11, 12, 13, 2 },
				TextualSummary =
					"""
					A participant is a badminton player registered with the Badminton World Federation. Players specialize in one of the five forms of the game.
					Some properties related to players are:
					- Name.
					- Age.
					- Height.
					- Country.
					- Preferred hand.
					- World ranking.
					- World tour ranking.
					- Sponsors.
					"""
			}
		}
	};
}
