import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { Skeleton } from '@/components/ui/skeleton';
import { useParams } from "react-router-dom";

const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL;

interface UserMessage {
	id: string;
	sender: "User";
	text: string;
	timestamp: string;
	replyMessageUri?: string;
}

interface SystemMessage {
	id: string;
	sender: "System";
	text?: string; // This field should be non-empty only for the very first system's welcome message.
	timestamp: string;
	mappingText?: string;
	mappedItems?: MappedItem[];
	sparqlText?: string;
	sparqlQuery?: string;
	suggestItemsText?: string;
	suggestedItemsGrouped?: Record<string, SuggestedItem[]>;
}

interface MappedItem {
	iri: string;
	label: string;
	summary: string;
	mappedWords: string;
	mappedOrSuggested: "Mapped";
}

interface SuggestedItem {
	iri: string;
	label: string;
	summary: string;
	reason: string;
	mappedOrSuggested: "Suggested";
}

// Type guard to check if a message is a ReplyMessage
const isSystemMessage = (message: UserMessage | SystemMessage): message is SystemMessage => {
	return message.sender === "System";
};
const isSuggestedItem = (item: MappedItem | SuggestedItem): item is SuggestedItem => {
	return item.mappedOrSuggested === "Suggested";
}

function ConversationPage() {
	const [messages, setMessages] = useState<(SystemMessage | UserMessage)[]>([]);
	const [currentMessage, setCurrentMessage] = useState<string>("");
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>(null);
	const [selectedItemsForExpansion, setSelectedItemsForExpansion] = useState<SuggestedItem[]>([]);
	const [isSummaryDialogOpen, setIsSummaryDialogOpen] = useState<boolean>(false);
	const [selectedItemForSummary, setSelectedItemForSummary] = useState<{ item: SuggestedItem | MappedItem, parentMessageId: string } | null>(null);
	const [isFetchingMessages, setIsFetchingMessages] = useState<boolean>(true);
	const [error, setError] = useState<string | null>(null);
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const { conversationId } = useParams<{ conversationId: string }>();
	const [mostRecentReplyMessageId, setMostRecentReplyMessageId] = useState<string | null>(null);
	const [summaryError, setSummaryError] = useState<string | null>(null);
	const [mostRecentUserMessage, setMostRecentUserMessage] = useState<string | null>(null);

	const fetchMessages = async () => {
		try {
			setIsFetchingMessages(true);
			setError(null); // Clear previous error if there was one.
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`);
			if (!response.ok) {
				console.error("Failed to fetch messages.")
				console.error("Response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching messages.");
			}
			const fetchedMessages = await response.json();
			console.log("Fetched messages in the conversation.");
			console.log(fetchedMessages);
			setMessages(fetchedMessages);

			// Look for the most recent system message.
			for (let i = fetchedMessages.length - 1; i >= 0; i--) {
				if (isSystemMessage(fetchedMessages[i])) {
					setMostRecentReplyMessageId(fetchedMessages[i].id);
					break;
				}
			}
			// Look for the most recent user message.
			for (let i = fetchedMessages.length - 1; i >= 0; i--) {
				if (!isSystemMessage(fetchedMessages[i])) {
					setMostRecentUserMessage(fetchedMessages[i].text);
					break;
				}
			}

		} catch (error) {
			console.error(error);
			setError("Failed to retrieve messages in the conversation.");
		} finally {
			setIsFetchingMessages(false);
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

		const messageToSend = currentMessage;
		const userMessage: UserMessage = {
				id: `UserMessage-${new Date().toString()}`, // ID is generated on the back end. Will be set later.
				sender: "User",
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
		setMostRecentUserMessage(messageToSend);

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
			userMessage.replyMessageUri = postUserMsgData.replyMessageUri;

			// Do another fetch to get the reply to user's message.
			console.log(`Fetching a reply from ${BACKEND_API_URL}${userMessage.replyMessageUri}`);
			const getReplyMsgResponse = await fetch(`${BACKEND_API_URL}${userMessage.replyMessageUri}`);
			if (!getReplyMsgResponse.ok) {
				throw new Error("Failed to get the reply message from the back end.")
			} else {
				console.log("Reply message fetched successfully.");
			}
			const replyMsgData = await getReplyMsgResponse.json();
			console.log(`getReplyMsgData: ${JSON.stringify(replyMsgData)}`);
			/*const replyMessage: SystemMessage = {
				id: getReplyMsgData.id,
				sender: getReplyMsgData.sender,
				text: getReplyMsgData.text,
				timestamp: getReplyMsgData.timestamp,
				mappingText: getReplyMsgData.mappingText,
				mappedItems: getReplyMsgData.mappedItems,
				sparqlText: getReplyMsgData.sparqlText,
				sparqlQuery: getReplyMsgData.sparqlQuery,
				suggestItemsText: getReplyMsgData.suggestItemsText,
				suggestedItemsGrouped: getReplyMsgData.suggestedItemsGrouped
			};*/

			setMessages((prevMessages) => [
				...prevMessages, replyMsgData
			]);
			setMostRecentReplyMessageId(replyMsgData.id);

		} catch (error) {
			console.error("Error sending message:", error);
			setMessages((prevMessages) => [
				...prevMessages,
				{
					id: `app-error-${new Date().toLocaleString()}`,
					sender: "System",
					text: "Sorry, there was some kind of error. I could not come up with a suitable answer.",
					timestamp: new Date().toLocaleString()
				}
			]);
		}
	};

	const handleItemClick = async (item: SuggestedItem | MappedItem, parentMessageId: string) => {
		setSelectedItemForSummary({ item, parentMessageId });
		setIsSummaryDialogOpen(true);
		if (item.summary) {
			setSummaryError(null);
		} else {
			setSummaryError('Failed to load item summary. Please try again.');
		}
	};

	const handleAddItemToMessage = () => {
		if (selectedItemForSummary && isSuggestedItem(selectedItemForSummary.item) && !selectedItemsForExpansion.some(item => item.iri === selectedItemForSummary.item.iri)) {
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
				{isFetchingMessages ? (
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
						className={`flex ${msg.sender === "User" ? "justify-end" : "justify-start"}`}
					>
						<Card
							className={`${
								msg.sender === "User" ? "bg-blue-100" : "bg-gray-100"
							} max-w-2xl`}
						>
							<CardContent className="p-3">
								<p className={`text-xs mt-1 ${msg.sender === "User" ? "text-right text-gray-600" : "text-left text-gray-500"}`}>
									{new Date(msg.timestamp).toLocaleString()}
								</p>

								{msg.sender === 'User' && (
									<p>{msg.text}</p>
								)}

								{isSystemMessage(msg) && (
									<>
										{/* If msg.text is available, it is the first system message. */}
										{msg.text && (<p>{msg.text}</p>)}

										{/* Mapped items */}
										{msg.mappingText && msg.mappedItems && msg.mappedItems.length > 0 && (
											<div className="mt-2">
												<p className="font-semibold text-sm">{msg.mappingText}</p>
												<ul className="list-disc list-inside mt-1">
													{msg.mappedItems.map((item) => {
														const isSelected = selectedItemsForExpansion.some(
															(selectedItem) => selectedItem.iri === item.iri
														);
														return (
															<li key={`${msg.id}-${item.iri}`} className="flex items-center space-x-1">
																<Button
																	variant="link"
																	onClick={() => handleItemClick(item, msg.id)}
																	className="p-0 h-auto text-sm text-blue-600 underline cursor-pointer"
																>
																	{item.label}
																</Button>
																{isSelected && (
																	<svg
																		xmlns="http://www.w3.org/2000/svg"
																		className="h-4 w-4 text-green-500"
																		viewBox="0 0 20 20"
																		fill="currentColor"
																	>
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

										{/* Sparql query */}
										{msg.sparqlText && (
											<div className="mt-2">
												<p className="font-semibold text-sm">{msg.sparqlText}</p>
												<pre className="mt-2 p-2 bg-gray-50 rounded text-sm overflow-x-auto">
													<code>{msg.sparqlQuery}</code>
												</pre>
											</div>
										)}

										{/* Suggested items */}
										{msg.suggestItemsText && msg.suggestedItemsGrouped && Object.keys(msg.suggestedItemsGrouped).length > 0 && (
											<div className="mt-4">
												<p className="font-semibold text-sm">{msg.suggestItemsText}</p>
												{Object.entries(msg.suggestedItemsGrouped).map(([mappedWords, items]) => (
													<div key={mappedWords} className="mt-2">
														<p className="text-sm font-medium italic">{mappedWords === "" ? "More items to consider" : `Expand "${mappedWords}"`}:</p>
														<ul className="list-disc list-inside ml-4 mt-1">
															{items.map((item) => {
																const isSelected = selectedItemsForExpansion.some(
																	(selectedItem) => selectedItem.iri === item.iri
																);
																return (
																	<li key={item.iri} className="flex items-center space-x-1">
																		<Button
																			variant="link"
																			onClick={() => handleItemClick(item, msg.id)}
																			className="p-0 h-auto text-sm text-blue-600 underline cursor-pointer"
																		>
																			{item.label}
																		</Button>
																		{isSelected && (
																			<svg
																				xmlns="http://www.w3.org/2000/svg"
																				className="h-4 w-4 text-green-500"
																				viewBox="0 0 20 20"
																				fill="currentColor"
																			>
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
												))}
											</div>
										)}
									</>
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

			{mostRecentUserMessage && (
        <Card className="mt-4 p-3 bg-blue-50 border-blue-200">
          <CardContent className="p-0">
            <p className="text-sm font-medium">Your current message: {mostRecentUserMessage}</p>
          </CardContent>
        </Card>
      )}

			<div className="flex mt-4 space-x-2">
				<Input
					placeholder={suggestedMessage ? "Modify suggestion or send as is..." : "Type your message..."}
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
						{summaryError && (
							<DialogDescription className="text-red-500">
								Error: {summaryError}
							</DialogDescription>
						)}
					</DialogHeader>
					<div className="py-4">
						{selectedItemForSummary?.item.summary && !summaryError && (
              <>
                <p>{selectedItemForSummary.item.summary}</p>
                {isSuggestedItem(selectedItemForSummary.item) ? (
                  <p className="mt-2 text-sm text-gray-700 font-semibold">Reason for suggestion:<br/><span className="font-normal">{selectedItemForSummary.item.reason}</span></p>
                ) : selectedItemForSummary.item.mappedWords ? (
                  <p className="mt-2 text-sm text-gray-700 font-semibold">Mapped from: <span className="font-normal">{selectedItemForSummary.item.mappedWords}</span></p>
                ) : null}
              </>
            )}
						{selectedItemForSummary && isSelectedItemFromMostRecentAnswer ? (
              // Check if the item is a suggested item
              isSuggestedItem(selectedItemForSummary.item) ? (
                // If it's a suggested item, show the button to add it to the message
                !selectedItemsForExpansion.some(item => item.iri === selectedItemForSummary.item.iri) ? (
                  <Button onClick={() => handleAddItemToMessage()} className="mt-4" disabled={!!summaryError}>
                    Add item to my message
                  </Button>
                ) : (
                  <p className="mt-4 text-sm text-green-600 font-semibold">
                    This item has been added to your message.
                  </p>
                )
              ) : (
                // If it's a mapped item, do not show the button.
                <></>
              )
            ) : (
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