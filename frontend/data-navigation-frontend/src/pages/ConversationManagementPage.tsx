import { useState, useEffect } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Trash2, FolderOpen } from "lucide-react"; // Example icons from lucide-react

interface Conversation {
	id: string;
	title: string; // Or a summary of the initial question
	lastUpdated: string;
	dataSpecificationName: string;
}

function ConversationManagementPage() {
	const [conversations, setConversations] = useState<Conversation[]>([]);

	useEffect(() => {
		// Simulate fetching conversations from backend 
		const fetchConversations = async () => {
			try {
				const response = await fetch("/api/conversations");
				if (!response.ok) {
					throw new Error("Failed to fetch conversations");
				}
				const data = await response.json();
				setConversations(data);
			} catch (error) {
				console.error("Error fetching conversations:", error);
				// Fallback or error message
				setConversations([
					{ id: "conv1", title: "Tournaments from Czech Republic", lastUpdated: "2025-06-30T10:00:00Z", dataSpecificationName: "Badminton Spec" },
					{ id: "conv2", title: "Player rankings", lastUpdated: "2025-06-29T15:30:00Z", dataSpecificationName: "Badminton Spec" },
				]);
			}
		};
		fetchConversations();
	}, []);

	const handleDeleteConversation = async (conversationId: string) => {
		try {
			const response = await fetch(`/api/conversations/${conversationId}`, {
				method: "DELETE",
			});
			if (!response.ok) {
				throw new Error("Failed to delete conversation");
			}
			setConversations(conversations.filter(conv => conv.id !== conversationId));
		} catch (error) {
			console.error("Error deleting conversation:", error);
			// Handle error
		}
	};

	const handleOpenConversation = (conversationId: string) => {
		// Redirect to the conversation page with the specific conversation loaded
		// In a real application, you"d load the conversation state based on the ID
		console.log(`Opening conversation: ${conversationId}`);
		// Example: navigate(`/conversation/${conversationId}`);
	};

	return (
		<div className="p-4">
			<h2 className="text-2xl font-bold mb-4">Manage Conversations</h2>
			{conversations.length === 0 ? (
				<p>No conversations found.</p>
			) : (
				<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
					{conversations.map((conv) => (
						<Card key={conv.id}>
							<CardHeader>
								<CardTitle>{conv.title}</CardTitle>
								<p className="text-sm text-gray-500">Data Spec: {conv.dataSpecificationName}</p>
								<p className="text-xs text-gray-400">Last updated: {new Date(conv.lastUpdated).toLocaleString()}</p>
							</CardHeader>
							<CardContent className="flex justify-end space-x-2">
								<Button variant="outline" size="sm" onClick={() => handleOpenConversation(conv.id)}>
									<FolderOpen className="mr-2 h-4 w-4" /> Open
								</Button>
								<Button variant="destructive" size="sm" onClick={() => handleDeleteConversation(conv.id)}>
									<Trash2 className="mr-2 h-4 w-4" /> Delete
								</Button>
							</CardContent>
						</Card>
					))}
				</div>
			)}
		</div>
	);
}

export default ConversationManagementPage;