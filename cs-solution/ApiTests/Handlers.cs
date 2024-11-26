using System.Text.Json;

namespace ApiTests
{
	public class Handlers
	{
		public static string PostUserPrompt(string prompt)
		{
			Console.WriteLine($"Received prompt: {prompt}");
			return $"Backend has received the user's prompt: {prompt}";
		}

		public static string GetChatHistory(int chatId)
		{
			Console.WriteLine($"Chat history with ID = {chatId} was requested.");
			List<ChatMessage> messages = new List<ChatMessage>();
			messages.Add(new ChatMessage("bot-message", $"Conversation #{chatId}: bot message 1."));
			messages.Add(new ChatMessage("user-message", $"Conversation #{chatId}: user message 1."));
			messages.Add(new ChatMessage("bot-message", $"Conversation #{chatId}: bot message 2."));
			messages.Add(new ChatMessage("user-message", $"Conversation #{chatId}: user message 2."));
			messages.Add(new ChatMessage("bot-message", $"Conversation #{chatId}: bot message 3."));
			return JsonSerializer.Serialize(messages);
		}
	}
}
