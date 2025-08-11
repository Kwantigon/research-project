using DataSpecificationNavigationBackend.Model;

namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO.Transformer;

public class SuggestionsTransformer
{
	public SuggestionsDTO TransformSuggestedProperties(
			IEnumerable<DataSpecificationPropertySuggestion> suggestions,
			DataSpecificationSubstructure substructure)
	{
		var directConnections = new List<GroupedSuggestionsDto>();
		var indirectConnections = new List<GroupedSuggestionsDto>();

		foreach (var classItem in substructure.ClassItems)
		{
			List<ArrowSuggestionDto> relevant = suggestions
				.Where(suggestion => suggestion.DomainItemIri == classItem.Iri || suggestion.RangeItemIri == classItem.Iri)
				.Select(suggestion =>
				{
					bool directionForward = suggestion.DomainItemIri == classItem.Iri;
					string otherClass;
					if (directionForward)
					{
						if (suggestion.RangeItem is null)
						{
							otherClass = suggestion.RangeItemIri;
						}
						else
						{
							otherClass = suggestion.RangeItem.Label;
						}
					}
					else
					{
						otherClass = suggestion.DomainItem.Label;
					}

					string connection = directionForward
						? $"→ {suggestion.Item.Label} → {otherClass}"
						: $"← {suggestion.Item.Label} ← {otherClass}";

					return new ArrowSuggestionDto(suggestion.ItemIri, suggestion.Item.Label, connection, suggestion.ReasonForSuggestion, suggestion.Item.Summary);
				})
				.ToList();

			if (relevant.Any())
			{
				directConnections.Add(new GroupedSuggestionsDto(classItem.Label, relevant));
			}
		}

		indirectConnections = suggestions
				.Where(s => !substructure.ClassItems.Any(item => s.DomainItemIri == item.Iri)
								 && !substructure.ClassItems.Any(item => s.RangeItemIri == item.Iri))
				.GroupBy(s => s.DomainItem.Label)
				.Select(group => new GroupedSuggestionsDto(
						group.Key,
						group.Select(s =>
								new ArrowSuggestionDto(
										s.ItemIri,
										s.Item.Label,
										$"→ {s.Item.Label} → {(s.RangeItem != null ? s.RangeItem.Label : s.RangeItemIri)}",
										s.ReasonForSuggestion,
										s.Item.Summary
								)
						).ToList()
				))
				.ToList();

		return new SuggestionsDTO
		{
			DirectConnections = directConnections,
			IndirectConnections = indirectConnections
		};
	}
}

