import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Skeleton } from '@/components/ui/skeleton';
import { useParams } from "react-router-dom";

const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL;

// Define types for messages and items based on the specification
interface Message {

	id: string;
	type: "UserMessage" | "WelcomeMessage" | "ReplyMessage" | "SuggestedMessage";
	text: string;
	relatedItems?: DataSpecificationItem[];
	timestamp: string;
}

interface DataSpecificationItem {
	iri: string;
	label: string;
	type: "Class" | "ObjectProperty" | "DatatypeProperty";
	summary?: string;
}

function ConversationPage() {
	const [messages, setMessages] = useState<Message[]>([]);
	const [currentMessage, setCurrentMessage] = useState<string>("");
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>(null);
	const [selectedItemsForExpansion, setSelectedItemsForExpansion] = useState<DataSpecificationItem[]>([]);
	const [isSummaryDialogOpen, setIsSummaryDialogOpen] = useState<boolean>(false);
	const [selectedItemForSummary, setSelectedItemForSummary] = useState<DataSpecificationItem | null>(null);
	const [isLoading, setIsLoading] = useState<boolean>(true);
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const { conversationId } = useParams<{ conversationId: string }>();

	const fetchMessages = async () => {
		try {
			setIsLoading(true);
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`);
			if (!response.ok) {
				console.error("Failed to fetch conversations.")
				console.error("Response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching conversations.");
			}
			const messages = await response.json();
			console.log(messages);
			setMessages(messages);
		} catch (error) {
			console.error(error);
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		fetchMessages();
	}, []);

	useEffect(() => {
		if (messagesEndRef.current) {
			messagesEndRef.current.scrollTop = messagesEndRef.current.scrollHeight;
		}
	}, [messages]);

	const handleSendMessage = async () => {
		if (currentMessage.trim() === "" && suggestedMessage === null) return;

		const messageToSend = suggestedMessage || currentMessage;

		// Add user's message to conversation
		setMessages((prevMessages) => [
			...prevMessages,
			{
				id: `UserMessage-${Date.now()}`,
				type: "UserMessage",
				text: messageToSend,
				timestamp: Date.now().toString()
			}
		]);

		// Clear input and suggested message
		setCurrentMessage("");
		setSuggestedMessage(null);
		setSelectedItemsForExpansion([]);

		try {
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({
					conversationId: conversationId,
					userQuestion: messageToSend
				}),
			});

			if (!response.ok) {
				throw new Error("Failed to get response from application");
			}

			const data = await response.json();
			const { sparqlQuery, relatedItems } = data; // Mocking backend response

			// Add application's reply message
			setMessages((prevMessages) => [
				...prevMessages,
				{
					id: `app-${Date.now()}`,
					type: "ReplyMessage",
					text: `The data you want can be retrieved using the following Sparql query: ${sparqlQuery}`,
					isSparqlQuery: true,
					relatedItems: relatedItems.map((item: any) => ({ ...item, isAddedToQuestion: false })), // Initialize isAddedToQuestion 
					timestamp: new Date().toISOString(),
				},
			]);
		} catch (error) {
			console.error("Error sending message:", error);
			// Handle error, e.g., display an error message to the user
			setMessages((prevMessages) => [
				...prevMessages,
				{
					id: `app-error-${Date.now()}`,
					type: "ReplyMessage",
					text: "I could not find any suitable matches from the data specification and cannot help you construct a query for this. Please try rephrasing your question.",
					timestamp: new Date().toISOString(),
				},
			]);
		}
	};

	const handleItemClick = (item: DataSpecificationItem) => {
		setSelectedItemForSummary(item);
		setIsSummaryDialogOpen(true);
	};

	const handleAddItemToQuestion = () => {
		if (selectedItemForSummary) {
			const updatedSelectedItems = [...selectedItemsForExpansion, selectedItemForSummary];
			setSelectedItemsForExpansion(updatedSelectedItems);
			setIsSummaryDialogOpen(false);

			// Simulate API call to get suggested message
			fetch("/api/conversation/suggest-message", {
				method: "POST",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({
					conversationId: "current-conversation-id",
					currentQuestion: messages[messages.length - 1]?.text, // Get the last user question
					itemsToAdd: updatedSelectedItems.map(item => item.iri),
				}),
			})
			.then(res => res.json())
			.then(data => {
				setSuggestedMessage(data.suggestedMessage); // Set the suggested message in the chat box
			})
			.catch(error => console.error("Error generating suggested message:", error));
		}
	};

	/*
	To display message timestamps, add this to CardContent:
		<p className={`text-xs mt-1 ${msg.sender === "user" ? "text-right text-gray-600" : "text-left text-gray-500"}`}>
				{new Date(msg.timestamp).toLocaleString()}
		</p>
	*/

	return (
		<div className="flex flex-col h-full p-4">
			<div ref={messagesEndRef} className="flex-1 overflow-y-auto border rounded-md p-4 space-y-4">
				{isLoading ? (
					// Display skeleton loaders while loading
					<div className="space-y-4">
						<Skeleton className="h-10 w-3/4" />
						<Skeleton className="h-10 w-1/2 ml-auto" />
						<Skeleton className="h-10 w-2/3" />
					</div>
				) : 
				(messages.map((msg) => (
					<div
						key={msg.id}
						className={`flex ${msg.type === "UserMessage" ? "justify-end" : "justify-start"}`}
					>
						<Card
							className={`${
								msg.type === "UserMessage" ? "bg-blue-100" : "bg-gray-100"
							} max-w-2xl`}
						>
							<CardContent className="p-3">
								<p>{msg.text}</p>
								{/*msg.isSparqlQuery && (
									<pre className="mt-2 p-2 bg-gray-50 rounded text-sm overflow-x-auto">
										<code>{msg.text.split("Sparql query: ")[1]}</code>
									</pre>
								)*/}
								{msg.relatedItems && msg.relatedItems.length > 0 && (
									<div className="mt-2">
										<p className="font-semibold text-sm">Some words in your question can be expanded upon. You can click on each item to see more information about it.</p>
										<ul className="list-disc list-inside mt-1">
											{msg.relatedItems.map((item) => (
												<li key={item.iri}>
													<Button variant="link" onClick={() => handleItemClick(item)} className="p-0 h-auto text-sm">
														{item.label}
													</Button>
												</li>
											))}
										</ul>
									</div>
								)}
							</CardContent>
						</Card>
					</div>
				)))}
			</div>

			{suggestedMessage && (
				<Card className="mt-4 p-3 bg-yellow-50 border-yellow-200">
					<CardContent className="p-0">
						<p className="text-sm font-medium">Suggested message: {suggestedMessage}</p>
					</CardContent>
				</Card>
			)}

			<div className="flex mt-4 space-x-2">
				<Input
					placeholder={suggestedMessage ? "Modify suggestion or send as is..." : "Ask your question..."}
					value={suggestedMessage || currentMessage} // Display suggested message or current input
					onChange={(e) => {
						if (suggestedMessage) {
							setSuggestedMessage(e.target.value); // Allow modification of suggested message
						} else {
							setCurrentMessage(e.target.value);
						}
					}}
					onKeyPress={(e) => {
						if (e.key === "Enter") {
							handleSendMessage();
						}
					}}
				/>
				<Button onClick={handleSendMessage}>SEND</Button>
			</div>

			<Dialog open={isSummaryDialogOpen} onOpenChange={setIsSummaryDialogOpen}>
				<DialogContent>
					<DialogHeader>
						<DialogTitle>Summary of "{selectedItemForSummary?.label}"</DialogTitle>
					</DialogHeader>
					<div className="py-4">
						<p>{selectedItemForSummary?.summary}</p>
						{selectedItemForSummary && !selectedItemsForExpansion.some(item => item.iri === selectedItemForSummary.iri) && ( // Only show "Add item" if not already selected
							<Button onClick={handleAddItemToQuestion} className="mt-4">
								Add item to my question
							</Button>
						)}
					</div>
				</DialogContent>
			</Dialog>
		</div>
	);
}

export default ConversationPage;