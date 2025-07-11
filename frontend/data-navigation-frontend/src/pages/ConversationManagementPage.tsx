import { useState, useEffect } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Trash2, FolderOpen } from "lucide-react";
import { Skeleton } from '@/components/ui/skeleton';
import { useNavigate } from "react-router-dom";

const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL;

interface Conversation {
	id: string;
	title: string;
	dataSpecificationName: string;
	lastUpdated: string;
}

function ConversationManagementPage() {
	const [conversations, setConversations] = useState<Conversation[]>([]);
	const [isLoading, setIsLoading] = useState<boolean>(true);
	const [error, setError] = useState<string | null>(null);
	const navigate = useNavigate();

	const fetchConversations = async () => {
		try {
			setIsLoading(true);
			setError(null); // Clear previous error if there was one.
			const response = await fetch(`${BACKEND_API_URL}/conversations`);
			if (!response.ok) {
				console.error("Failed to fetch conversations.")
				console.error("Response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching conversations.");
			}
			const data = await response.json();
			setConversations(data);
		} catch (error) {
			console.error(error);
			setError("There was an error while retrieving conversations from the back end. See the console for more info.");
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		fetchConversations();
	}, []);

	const handleDeleteConversation = async (conversationId: string) => {
		try {
			// Optimistically remove the conversation from the UI.
				setConversations(conversations.filter(conv => conv.id !== conversationId));
				setError(null); // Clear previous error if there was one.

			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}`, { method: "DELETE" });
			if (!response.ok) {
				console.error("Failed to delete the conversation.")
				console.error("Response status: " + response.status);
				console.error(response.body);
				throw new Error("Error deleting conversation.");
			}
			setConversations(conversations.filter(conv => conv.id !== conversationId));
		} catch (error) {
			console.error("Error deleting conversation:", error);
			setError("There was an error while deleting the conversation. See the console for more info.");
			
      // Re-fetch conversations to revert the optimistic update.
      fetchConversations();
		}
	};

	const handleOpenConversation = (conversationId: string) => {
		console.log(`Navigating to: /conversation/${conversationId}`);
		navigate(`/conversation/${conversationId}`);
	};

	return (
		<div className="p-4">
			<h2 className="text-2xl font-bold mb-4">Manage Conversations</h2>
			{isLoading ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <Skeleton className="h-40 w-full rounded-lg" />
          <Skeleton className="h-40 w-full rounded-lg" />
          <Skeleton className="h-40 w-full rounded-lg" />
        </div>
      ) : error ? (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
          <strong className="font-bold">Error!</strong>
          <span className="block sm:inline"> {error}</span>
        </div>
      ) : conversations.length === 0 ? (
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