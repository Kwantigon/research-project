﻿namespace Backend.DTO;

public class PostConversationMessagesDTO
{
	public required DateTime TimeStamp { get; set; }

	public string? TextValue { get; set; }

	public bool UserModifiedPreviewMessage { get; set; } = false;
}
