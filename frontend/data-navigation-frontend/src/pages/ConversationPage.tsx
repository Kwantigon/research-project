import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
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
	summaryEndpoint: string;
}

function ConversationPage() {
	const [messages, setMessages] = useState<Message[]>([]);
	const [currentMessage, setCurrentMessage] = useState<string>("");
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>(null);
	const [selectedItemsForExpansion, setSelectedItemsForExpansion] = useState<DataSpecificationItem[]>([]);
	const [isSummaryDialogOpen, setIsSummaryDialogOpen] = useState<boolean>(false);
	const [selectedItemForSummary, setSelectedItemForSummary] = useState<{ item: DataSpecificationItem, parentMessageId: string } | null>(null);
	const [isLoading, setIsLoading] = useState<boolean>(true);
	const [error, setError] = useState<string | null>(null);
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const { conversationId } = useParams<{ conversationId: string }>();
	const [mostRecentReplyMessageId, setMostRecentReplyMessageId] = useState<string | null>(null);
	const [isSummaryLoading, setIsSummaryLoading] = useState<boolean>(false);
		const [summaryError, setSummaryError] = useState<string | null>(null);

	const fetchMessages = async () => {
		try {
			setIsLoading(true);
			setError(null); // Clear previous error if there was one.
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`);
			if (!response.ok) {
				console.error("Failed to fetch messages.")
				console.error("Response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching messages.");
			}
			const messages = await response.json();
			console.log(messages);
			setMessages(messages);

			// Look for the most recent message with a relatedItems list.
			for (let i = messages.length - 1; i >= 0; i--) {
				if (messages[i].relatedItems) {
					setMostRecentReplyMessageId(messages[i].id);
					break;
				}
			}
		} catch (error) {
			console.error(error);
			setError("Failed to retrieve messages in the conversation.");
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
		setError(null);

		const messageToSend = suggestedMessage || currentMessage;
		const userMessage: Message = {
				id: `UserMessage-${new Date().toString()}`, // ID is generated on the back end. Will be set later.
				type: "UserMessage",
				text: messageToSend,
				timestamp: new Date().toLocaleString()
			};

		// Add user's message to conversation
		setMessages((prevMessages) => [
			...prevMessages, userMessage
		]);

		// Clear input and suggested message
		setCurrentMessage("");
		setSuggestedMessage(null);
		setSelectedItemsForExpansion([]);

		try {
			const requestBody = JSON.stringify(
					{
						textValue: userMessage.text,
						userModifiedSuggestedMessage: !(userMessage.text === suggestedMessage)
					});
			console.log("Sending a POST request with body " + requestBody);
			const postUserMsgResponse = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: requestBody
			});

			if (!postUserMsgResponse.ok) {
				throw new Error("Failed to send user's message to the back end.");
			} else {
				console.log("User message POSTed successfully.");
			}
			const postUserMsgData = await postUserMsgResponse.json();
			console.log(`postUserMsgData: ${JSON.stringify(postUserMsgData)}`);
			userMessage.id = postUserMsgData.id; // Set the message ID that the back end generated.

			// Do another fetch to get the reply to user's message.
			console.log(`Fetching a reply from ${BACKEND_API_URL}${postUserMsgData.replyUri}`);
			const getReplyMsgResponse = await fetch(`${BACKEND_API_URL}${postUserMsgData.replyUri}`);
			if (!getReplyMsgResponse.ok) {
				throw new Error("Failed to get the reply message from the back end.")
			} else {
				console.log("Reply message fetched successfully.");
			}
			const getReplyMsgData = await getReplyMsgResponse.json();
			console.log(`getReplyMsgData: ${JSON.stringify(getReplyMsgData)}`);

			setMessages((prevMessages) => [
				...prevMessages,
				{
					id: getReplyMsgData.id,
					type: getReplyMsgData.type,
					text: getReplyMsgData.text,
					timestamp: getReplyMsgData.timestamp,
					relatedItems: getReplyMsgData.relatedItems
				}
			]);

			if (getReplyMsgData.relatedItems && getReplyMsgData.relatedItems.length > 0) {
				setMostRecentReplyMessageId(getReplyMsgData.id);
			} else {
				setMostRecentReplyMessageId(null);
			}
		} catch (error) {
			console.error("Error sending message:", error);
			setMessages((prevMessages) => [
				...prevMessages,
				{
					id: `app-error-${new Date().toLocaleString()}`,
					type: "ReplyMessage",
					text: "Sorry, there was some kind of error. I could not come up with a suitable answer.",
					timestamp: new Date().toLocaleString()
				},
			]);
		}
	};

	const handleItemClick = async (item: DataSpecificationItem, parentMessageId: string) => {
		setSelectedItemForSummary({ item, parentMessageId });
		setIsSummaryDialogOpen(true);
		if (item.summary) {
			// Summary is already present, no need to fetch it again.
			setIsSummaryLoading(false);
			setSummaryError(null);
			return;
		}

		// If summary is not present.
		setIsSummaryLoading(true);
		setSummaryError(null);
		try {
			const uri = encodeURI(`${BACKEND_API_URL}${item.summaryEndpoint}`);
			const response = await fetch(uri);
			if (!response.ok) {
				throw new Error(`Failed to fetch item summary: ${response.statusText}`);
			}
			const data = await response.json();
			item.summary = data.summary;
			//setSelectedItemForSummary(prev => prev ? { ...prev, item: { ...prev.item, summary: data.summary } } : null);
			setSelectedItemForSummary({item, parentMessageId}); // Not sure if I need to call set again here but calling it just in case.
		} catch (error) {
			console.error('Error fetching item summary:', error);
			setSummaryError('Failed to load item summary. Please try again.');
			setSelectedItemForSummary(prev => prev ? { ...prev, item: { ...prev.item, summary: 'Summary could not be loaded.' } } : null);
			// Not doing item.summary = "Summary could not be loaded." here because I want it to be fetched again upon clicking.
		} finally {
			setIsSummaryLoading(false);
		}
	};

	const handleAddItemToQuestion = () => {
		if (selectedItemForSummary && !selectedItemsForExpansion.some(item => item.iri === selectedItemForSummary.item.iri)) {
			const updatedSelectedItems = [...selectedItemsForExpansion, selectedItemForSummary.item];
			setSelectedItemsForExpansion(updatedSelectedItems);
			setIsSummaryDialogOpen(false);

			// Call back end API to get the suggested message.
			fetch(`${BACKEND_API_URL}/conversations/${conversationId}/user-selected-items`, {
				method: "PUT",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({
					//conversationId: "current-conversation-id",
					//currentQuestion: messages[messages.length - 1]?.text, // Get the last user question
					itemIriList: updatedSelectedItems.map(item => item.iri)
				})
			})
			.then(res => res.json())
			.then(data => {
				setSuggestedMessage(data.suggestedMessage); // Set the suggested message in the chat box
			})
			.catch(error => console.error("Error generating suggested message:", error));
		}
	};

	const isSelectedItemFromMostRecentAnswer = selectedItemForSummary?.parentMessageId === mostRecentReplyMessageId;

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
				) : error ? (
					<div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
						<strong className="font-bold">Error!</strong>
						<span className="block sm:inline"> {error}</span>
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
								<p className={`text-xs mt-1 ${msg.type === "UserMessage" ? "text-right text-gray-600" : "text-left text-gray-500"}`}>
									{new Date(msg.timestamp).toLocaleString()}
								</p>
								<p>{msg.text}</p>
								{/*msg.isSparqlQuery && (
									<pre className="mt-2 p-2 bg-gray-50 rounded text-sm overflow-x-auto">
										<code>{msg.text.split("Sparql query: ")[1]}</code>
									</pre>
								)*/}
								{msg.relatedItems && msg.relatedItems.length > 0 && (
									<div className="mt-2">
										<p className="font-semibold text-sm">Some parts in your question can be expanded upon. You can click on each item to see more information about it.</p>
										<ul className="list-disc list-inside mt-1">
											{msg.relatedItems.map((item) => {
													const isSelected = selectedItemsForExpansion.some(
														(selectedItem) => selectedItem.iri === item.iri
													);
													return (
														<li key={item.iri} className="flex items-center space-x-1">
															<Button
																variant="link"
																onClick={() => handleItemClick(item, msg.id)}
																className="p-0 h-auto text-sm text-blue-600 cursor-pointer">
																{item.label}
															</Button>
															{isSelected && (
																<svg
																	xmlns="http://www.w3.org/2000/svg"
																	className="h-4 w-4 text-green-500"
																	viewBox="0 0 20 20"
																	fill="currentColor">
																	<path
																		fillRule="evenodd"
																		d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
																		clipRule="evenodd"
																	/>
																</svg>
															)}
														</li>
														);
													})}
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
					onKeyDown={(e) => {
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
						<DialogTitle>Summary of "{selectedItemForSummary?.item.label}"</DialogTitle>
						{isSummaryLoading && (
              <DialogDescription className="flex items-center text-blue-500">
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-blue-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Loading summary...
              </DialogDescription>
            )}
            {summaryError && (
              <DialogDescription className="text-red-500">
                Error: {summaryError}
              </DialogDescription>
            )}
					</DialogHeader>
					<div className="py-4">
						{selectedItemForSummary?.item.summary && !isSummaryLoading && !summaryError ? (
              <p>{selectedItemForSummary.item.summary}</p>
            ) : (
              !isSummaryLoading && !summaryError && <p>No summary available.</p>
            )}
						{selectedItemForSummary && isSelectedItemFromMostRecentAnswer ? (
							!selectedItemsForExpansion.some(item => item.iri === selectedItemForSummary.item.iri) ? (
								<Button onClick={() => handleAddItemToQuestion()} className="mt-4" disabled={isSummaryLoading}>
									Add item to my question
								</Button>
							) : (
								<p className="mt-4 text-sm text-green-600 font-semibold">
									This item has been added to your question.
								</p>
							)
						) : (
							// Show reminder text if not from most recent answer
							<p className="mt-4 text-sm text-gray-500 italic">
								This item cannot be used because it is not from the most recent answer.
							</p>
						)}
					</div>
				</DialogContent>
			</Dialog>
		</div>
	);
}

export default ConversationPage;