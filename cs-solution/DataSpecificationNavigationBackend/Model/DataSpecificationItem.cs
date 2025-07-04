﻿using System.Text.Json.Serialization;

namespace DataspecNavigationHelper.Model;

public class DataSpecificationItem
{
	[JsonConstructor]
	public DataSpecificationItem(string iri, string label, ItemType type)
	{
		Iri = iri;
		Label = label;
		Type = type;
	}

	[JsonPropertyName("iri")]
	public string Iri { get; init; }

	[JsonPropertyName("label")]
	public string Label { get; init; }

	[JsonPropertyName("type")]
	public ItemType Type { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter<ItemType>))]
public enum ItemType
{
	Class, // Entities.
	ObjectProperty, // Relationship between entities.
	DatatypeProperty // Attributes of entities.
}
