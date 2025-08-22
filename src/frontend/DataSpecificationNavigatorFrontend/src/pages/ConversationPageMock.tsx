import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { Skeleton } from '@/components/ui/skeleton';
import { useParams } from "react-router-dom";
import MessagesList from "./MessagesList";
import { Checkbox } from "@radix-ui/react-checkbox";
import { Label } from "@radix-ui/react-label";

const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL;

export interface Message {
	id: string;
	text: string;
	timestamp: string;
	type: "WelcomeMessage" | "UserMessage" | "ReplyMessage";
}
export interface WelcomeMessage extends Message {
	dataSpecificationSummary: string;
	suggestedMessage: string;
	type: "WelcomeMessage";
}
export interface UserMessage extends Message {
	replyMessageUri?: string;
	type: "UserMessage";
}
export interface ReplyMessage extends Message {
	mappedItems: MappedItem[];
	sparqlQuery?: string;
	suggestions?: Suggestions;
	type: "ReplyMessage";
}

export interface MappedItem {
	iri: string;
	label: string;
	summary: string;
	mappedPhrase: string;
	startIndex: number;
	endIndex: number;
}

export interface SuggestedProperty {
	iri: string;
	label: string;
	connection: string;
	reason: string;
	summary: string;
	type: "DatatypeProperty" | "ObjectProperty"
}
const isDatatypeProperty = (property: SuggestedProperty) => {
	return property.type === "DatatypeProperty";
}

export interface GroupedSuggestions {
	domain: string;
	suggestions: SuggestedProperty[];
}

export interface Suggestions {
	directConnections: GroupedSuggestions[];
	indirectConnections: GroupedSuggestions[];
}

export interface SubstructureDatatypeProperty {
	iri: string;
	label: string;
	domain: string;
	range: string;
}

export interface SubstructureObjectProperty {
	iri: string;
	label: string;
	domain: string;
	range: string;
}

export interface SubstructureClass {
	iri: string;
	label: string;
	isSelectTarget: boolean;
	objectProperties: SubstructureObjectProperty[];
	datatypeProperties: SubstructureDatatypeProperty[];
}

export interface DataSpecificationSubstructure {
	classItems: SubstructureClass[];
}

export interface SelectedSuggestedItem extends SuggestedProperty {
	isOptional: boolean;
	filterExpression: string;
}

function renderMessageWithMappedItems(
	text: string,
	mappedItems: MappedItem[],
	onMappedItemClick: (item: MappedItem) => void
) {
	// sort spans by start index (just in case backend sends unordered)
	const sortedByStartPositions = [...mappedItems].sort((a, b) => a.startIndex - b.startIndex);

	const elements: React.ReactNode[] = [];
	let lastIndex = 0;

	sortedByStartPositions.forEach((item, i) => {
		// push the text before this item.
		if (item.startIndex > lastIndex) {
			elements.push(<span key={`text-${i}`}>{text.slice(lastIndex, item.startIndex)}</span>);
		}

		// push the clickable words.
		elements.push(
			<button
				key={`span-${i}`}
				onClick={() => onMappedItemClick(item)}
				className="text-blue-600 underline hover:text-blue-800"
			>
				{text.slice(item.startIndex, item.endIndex)}
			</button>
		);

		lastIndex = item.endIndex;
	});

	// push any trailing text after the last item.
	if (lastIndex < text.length) {
		elements.push(<span key="text-end">{text.slice(lastIndex)}</span>);
	}

	return <>{elements}</>;
}

const mockMessages: (WelcomeMessage | UserMessage | ReplyMessage)[] = [
	{
		id: "welcome-1",
		type: "WelcomeMessage",
		text: "Welcome to the OWL Data Chatbot!",
		timestamp: new Date().toISOString(),
		dataSpecificationSummary: "This data specification describes various renewable energy concepts.",
		suggestedMessage: "Ask about wind power or solar energy."
	},
	{
		id: "user-1",
		type: "UserMessage",
		text: "Can you provide more on wind power?",
		timestamp: new Date().toISOString(),
		replyMessageUri: "/conversations/1/messages/reply-1"
	},
	{
		id: "reply-1",
		type: "ReplyMessage",
		text: "Here are some suggestions based on your query:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#WindPower",
				label: "Wind Power",
				summary: "Wind power is the conversion of wind energy into a useful form of energy.",
				mappedPhrase: "wind power",
				startIndex: 24,
				endIndex: 34
			}
		],
		sparqlQuery: "SELECT ?s WHERE { ?s ?p ?o }",
		suggestions: {
			directConnections: [
				{
					domain: "RenewableEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#WindTurbine",
							label: "Wind Turbine",
							connection: "→ part of → Wind Power",
							reason: "Directly related component",
							summary: "A device that converts wind energy into electricity.",
							type: "ObjectProperty"
						}
					]
				}
			],
			indirectConnections: [
				{
					domain: "EnergyPolicy",
					suggestions: [
						{
							iri: "http://example.org/ontology#GovernmentSubsidy",
							label: "Government Subsidy",
							connection: "← influences ← Wind Power",
							reason: "Indirect effect through policy",
							summary: "Financial support by the government to encourage wind energy adoption.",
							type: "DatatypeProperty"
						}
					]
				}
			]
		}
	},
	{
		id: "user-2",
		type: "UserMessage",
		text: "Can you provide more on wind power?",
		timestamp: new Date().toISOString(),
		replyMessageUri: "/conversations/1/messages/reply-2"
	},
	{
		id: "reply-2",
		type: "ReplyMessage",
		text: "Here are some suggestions based on your query:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#WindPower",
				label: "Wind Power",
				summary: "Wind power is the conversion of wind energy into a useful form of energy.",
				mappedPhrase: "wind power",
				startIndex: 24,
				endIndex: 34
			}
		],
		sparqlQuery: "SELECT ?s WHERE { ?s ?p ?o }",
		suggestions: {
			directConnections: [
				{
					domain: "RenewableEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#WindTurbine",
							label: "Wind Turbine",
							connection: "→ part of → Wind Power",
							reason: "Directly related component",
							summary: "A device that converts wind energy into electricity.",
							type: "ObjectProperty"
						}
					]
				}
			],
			indirectConnections: [
				{
					domain: "EnergyPolicy",
					suggestions: [
						{
							iri: "http://example.org/ontology#GovernmentSubsidy",
							label: "Government Subsidy",
							connection: "← influences ← Wind Power",
							reason: "Indirect effect through policy",
							summary: "Financial support by the government to encourage wind energy adoption.",
							type: "DatatypeProperty"
						}
					]
				}
			]
		}
	},
];

const mockSubstructure: DataSpecificationSubstructure = {
	classItems: [
		{
			iri: "http://example.org/ontology#WindPower",
			label: "Wind Power",
			isSelectTarget: true,
			objectProperties: [
				{
					iri: "http://example.org/ontology#hasTurbine",
					label: "has Turbine",
					domain: "Wind Power",
					range: "Wind Turbine"
				},
				{
					iri: "http://example.org/ontology#hasLocation",
					label: "has Location",
					domain: "Wind Power",
					range: "Location"
				}
			],
			datatypeProperties: [
				{
					iri: "http://example.org/ontology#installedCapacity",
					label: "Installed Capacity",
					domain: "Wind Power",
					range: "float"
				}
			]
		},
		{
			iri: "http://example.org/ontology#WindTurbine",
			label: "Wind Turbine",
			isSelectTarget: false,
			objectProperties: [
				{
					iri: "http://example.org/ontology#locatedAt",
					label: "located At",
					domain: "Wind Turbine",
					range: "Location"
				}
			],
			datatypeProperties: [
				{
					iri: "http://example.org/ontology#capacity",
					label: "Capacity",
					domain: "Wind Turbine",
					range: "float"
				},
				{
					iri: "http://example.org/ontology#height",
					label: "Height",
					domain: "Wind Turbine",
					range: "float"
				}
			]
		},
		{
			iri: "http://example.org/ontology#Location",
			label: "Location",
			isSelectTarget: false,
			objectProperties: [],
			datatypeProperties: [
				{
					iri: "http://example.org/ontology#latitude",
					label: "Latitude",
					domain: "Location",
					range: "float"
				},
				{
					iri: "http://example.org/ontology#longitude",
					label: "Longitude",
					domain: "Location",
					range: "float"
				}
			]
		}
	]
};


function ConversationPageMock() {
	const { conversationId } = useParams<{ conversationId: string }>();
	const [messages, setMessages] = useState<Message[]>([]);
	const [dataSpecificationSubstructure, setDataSpecificationSubstructure] = useState<DataSpecificationSubstructure | null>(null);
	const [isFetchingSubstructure, setIsFetchingSubstructure] = useState<boolean>(false);
	const [substructureError, setSubstructureError] = useState<string | null>(null);
	const [showSubstructure, setShowSubstructure] = useState<boolean>(true);
	const [currentReplyMessage, setCurrentReplyMessage] = useState<ReplyMessage | null>(null);
	const [currentUserMessage, setCurrentUserMessage] = useState<UserMessage | null>(null);
	const [mappedItemSelectedForSummary, setMappedItemSelectedForSummary] = useState<MappedItem | null>(null);
	const [isMappedItemSummaryDialogOpen, setIsMappedItemSummaryDialogOpen] = useState<boolean>(false);
	const [suggestedPropertySelectedForSummary, setSuggestedPropertySelectedForSummary] = useState<{ property: SuggestedProperty, replyMsg: ReplyMessage } | null>(null);
	const [isSuggestedPropertySummaryDialogOpen, setIsSuggestedPropertySummaryDialogOpen] = useState<boolean>(false);
	const [suggestedPropertyAddAsOptional, setSuggestedPropertyAddAsOptional] = useState<boolean>(false);
	const [suggestedPropertyFilterExpression, setSuggestedPropertyFilterExpression] = useState<string>("");

	//
	const [userMessageInput, setUserMessageInput] = useState<string>("");
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>(null);
	const [selectedItemsForExpansion, setSelectedItemsForExpansion] = useState<SelectedSuggestedItem[]>([]);
	const [selectedItemForSummary, setSelectedItemForSummary] = useState<{ item: SuggestedProperty | MappedItem, parentMessageId: string } | null>(null);
	const [isFetchingMessages, setIsFetchingMessages] = useState<boolean>(true);
	const [error, setError] = useState<string | null>(null);
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const [mostRecentUserMessage, setMostRecentUserMessage] = useState<string | null>(null);
	const [isWaitingForReplyMessage, setIsWaitingForReplyMessage] = useState<boolean>(false);
	/*const [dataSubstructure, setDataSubstructure] = useState<DataSpecificationSubstructure | null>(mockSubstructure);
	const [isSubstructureLoading, setIsSubstructureLoading] = useState<boolean>(false);
	const [substructureError, setSubstructureError] = useState<string | null>(null);*/
	const [isFetchingSuggestedMessage, setIsFetchingSuggestedMessage] = useState<boolean>(false);

	const fetchMessages = async () => {
		setIsFetchingMessages(true);
		await new Promise(f => setTimeout(f, 500));
		setMessages(mockMessages);
		const data = mockMessages;
		console.log(data.length);
		// Look for the most recent reply message.
		if (data.length > 1) {
			let replyMsgFound: boolean = false;
			let userMsgFound: boolean = false;
			for (let i = data.length - 1; i >= 0; i--) {
				if (data[i].type == "ReplyMessage" && !replyMsgFound) {
					setCurrentReplyMessage(data[i] as ReplyMessage);
					replyMsgFound = true;
				}
				if (data[i].type == "UserMessage" && !userMsgFound) {
					setCurrentUserMessage(data[i] as UserMessage);
					userMsgFound = true;
				}
				if (replyMsgFound && userMsgFound) {
					break;
				}
			}

			if (!replyMsgFound || !userMsgFound) {
				console.log("ERROR: there is more than 1 message in the conversation but a reply message or an user message is missing.");
			}
		}
		setIsFetchingMessages(false);
	};

	const fetchSubstructure = async () => {
		console.log("Simulating substructure fetch.");
		setIsFetchingSubstructure(true);
		setSubstructureError(null);
		await new Promise(f => setTimeout(f, 1000));
		setDataSpecificationSubstructure(mockSubstructure);
		setIsFetchingSubstructure(false);
	};

	useEffect(() => {
		fetchMessages();
		fetchSubstructure();
	}, []);
	/*if (currentUserMessage) {
		console.log("Current user message text: " + currentUserMessage.text);
	}
	if (currentReplyMessage) {
		console.log("Current reply message text: " + currentReplyMessage.text);
	}*/

	useEffect(() => {
		if (messagesEndRef.current) {
			messagesEndRef.current.scrollTop = messagesEndRef.current.scrollHeight;
		}
	}, [messages]);

	const handleSendMessage = async () => {
		if (userMessageInput.trim() === "") return;
		setError(null);
	};

	const handleSuggestedPropertyClick = async (property: SuggestedProperty, replyMsg: ReplyMessage) => {
		setSuggestedPropertySelectedForSummary({ property, replyMsg });
		setIsSuggestedPropertySummaryDialogOpen(true);
	};

	const addSelectedProperty = (property: SuggestedProperty, isOptional: boolean, filterExpression: string) => {
		const newItem: SelectedSuggestedItem = {
			...property,
			isOptional: isOptional,
			filterExpression: filterExpression
		};
		setSelectedItemsForExpansion([...selectedItemsForExpansion, newItem]);
	};

	const handleAddSuggestedProperty = () => {
		if (suggestedPropertySelectedForSummary) {
			addSelectedProperty(
				suggestedPropertySelectedForSummary.property,
				suggestedPropertyAddAsOptional,
				suggestedPropertyFilterExpression
			);
			// Reset the options.
			setSuggestedPropertyAddAsOptional(false);
			setSuggestedPropertyFilterExpression("");
		}
	}
	const handleRemoveSelectedProperty = (iri: string) => {
		const updatedSelectedItems = selectedItemsForExpansion.filter(item => item.iri !== iri);
		setSelectedItemsForExpansion(updatedSelectedItems);
	}

	const fetchSuggestedMessage = async () => {
		// Send the selectedItemsForExpansion array to backend.
	}

	const isSuggestionFromCurrentReply = (suggestedPropertySelectedForSummary?.replyMsg === currentReplyMessage);
	const suggestionIsSelected = (suggestedProperty: SuggestedProperty) => {
		return selectedItemsForExpansion.some(item => item.iri === suggestedProperty.iri);
	}
	return (
		<div className="flex h-full p-4 gap-4">
			{/* LEFT: Chat messages */}
			<div className={`flex flex-col ${showSubstructure ? "flex-[2]" : "flex-1"}`}>
				<div className="flex justify-end mb-2">
					<Button variant="outline" size="sm" onClick={() => setShowSubstructure(!showSubstructure)}>
						{showSubstructure ? "Hide mapped data specification items" : "Show mapped data specification items"}
					</Button>
				</div>

				{/* Cards with messages */}
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
					) : (
						<MessagesList
							messages={messages}
							selectedItemsForExpansion={selectedItemsForExpansion}
							onSuggestedItemClick={handleSuggestedPropertyClick}
						/>
					)}

					{/* Display a rotating circle while waiting for reply */}
					{isWaitingForReplyMessage && (
						<div className="flex justify-start">
							<Card className="bg-gray-100 max-w-2xl">
								<CardContent className="p-3">
									<div className="h-4 w-4 rounded-full border-2 border-gray-300 border-t-blue-500 animate-spin"></div>
									<div>Thinking....</div>
									<div>This might take a minute.</div>
								</CardContent>
							</Card>
						</div>
					)}
				</div>

				{/* Suggested message */}
				{isFetchingSuggestedMessage ? (
					<Card className="mt-4 p-3 flex justify-center items-center h-16 bg-yellow-50 border-yellow-200">
						<CardContent>
							<div className="h-8 w-8 rounded-full border-4 border-gray-300 border-t-yellow-500 animate-spin mr-2"></div>
							<p className="text-sm font-medium">Generating a suggested message...</p>
						</CardContent>
					</Card>
				) : suggestedMessage && (
					<Card className="mt-4 p-3 bg-yellow-50 border-yellow-200">
						<CardContent className="p-0">
							<p className="text-sm font-medium">Suggested message: {suggestedMessage}</p>
						</CardContent>
					</Card>
				)}

				{/* Current user message */}
				{currentUserMessage && (
					<Card className="mb-4 bg-blue-50">
						<CardContent className="p-4">
							{isWaitingForReplyMessage ? (
								// Current message as plain text.
								<p className="text-gray-800">{currentUserMessage.text}</p>
							) : (
								// Current message with clickable parts.
								<p className="text-gray-800">
									{currentUserMessage && renderMessageWithMappedItems(
										currentUserMessage.text,
										currentReplyMessage?.mappedItems ?? [],
										(mapped) => {
											setMappedItemSelectedForSummary(mapped);
											setIsMappedItemSummaryDialogOpen(true);
										}
									)}
								</p>
							)}
						</CardContent>
					</Card>
				)}

				{/* Input field and send message button */}
				<div className="flex mt-4 space-x-2">
					<Input
						placeholder={suggestedMessage ? "Modify suggestion or send as is..." : "Type your message..."}
						value={userMessageInput}
						onChange={(e) => setUserMessageInput(e.target.value)}
						onKeyDown={(e) => {
							if (e.key === "Enter") {
								handleSendMessage();
							}
						}}
					/>
					<Button onClick={handleSendMessage} disabled={isWaitingForReplyMessage}>SEND</Button>
				</div>
			</div>

			{/* RIGHT: Substructure sidebar (toggleable) */}
			{showSubstructure && (
				<div className="w-2/7 border rounded-md p-4 flex flex-col bg-gray-50">
					<h2 className="text-lg font-bold mb-2">Mapped data specification items</h2>
					{isFetchingSubstructure ? (
						<p>Loading items...</p>
					) : substructureError ? (
						<p className="text-red-500">{substructureError}</p>
					) : dataSpecificationSubstructure && dataSpecificationSubstructure.classItems.length > 0 ? (
						<div className="flex-1 overflow-y-auto space-y-4">
							{dataSpecificationSubstructure.classItems.map((classItem) => (
								<div key={classItem.iri} className="p-3 border-l-4 border-blue-500 bg-white shadow-sm rounded-md">
									<h3 className="text-base font-semibold text-blue-800">{classItem.label}</h3>
									<p className="text-xs text-gray-500 mb-1">IRI: {classItem.iri}</p>
									{classItem.objectProperties.length > 0 && (
										<div className="mt-2">
											<h4 className="text-sm font-medium text-gray-700">Object properties:</h4>
											<ul className="list-disc list-inside text-sm">
												{classItem.objectProperties.map((prop) => (
													<li key={prop.iri}>
														<span className="font-medium">{prop.label}</span>
														<span className="ml-1 text-xs text-gray-500">({prop.domain} → {prop.range})</span>
													</li>
												))}
											</ul>
										</div>
									)}


									{classItem.datatypeProperties.length > 0 && (
										<div className="mt-2">
											<h4 className="text-sm font-medium text-gray-700">Datatype properties:</h4>
											<ul className="list-disc list-inside text-sm">
												{classItem.datatypeProperties.map((prop) => (
													<li key={prop.iri}>
														<span className="font-medium">{prop.label}</span>
														<span className="ml-1 text-xs text-gray-500">({prop.range})</span>
													</li>
												))}
											</ul>
										</div>
									)}
								</div>
							))}
						</div>
					) : (
						<p className="text-gray-500">No data specification items to display.</p>
					)}
				</div>
			)}

			{/* Mapped item summary */}
			{mappedItemSelectedForSummary && (
				<Dialog open={isMappedItemSummaryDialogOpen}
					onOpenChange={setIsMappedItemSummaryDialogOpen}>
					<DialogContent>
						<DialogHeader>
							<DialogTitle>Summary of "{mappedItemSelectedForSummary?.label}"</DialogTitle>
						</DialogHeader>

						<div className="py-4">
							<p>{mappedItemSelectedForSummary?.summary}</p>
							<p className="mt-2 text-sm text-gray-700 font-semibold">
								Mapped from: <span className="font-normal">{mappedItemSelectedForSummary?.mappedPhrase}</span>
							</p>
						</div>
					</DialogContent>
				</Dialog>
			)}

			{/* Suggested property summary */}
			{suggestedPropertySelectedForSummary && (
				<Dialog open={isSuggestedPropertySummaryDialogOpen}
					onOpenChange={setIsSuggestedPropertySummaryDialogOpen}>
					<DialogContent>
						<DialogHeader>
							<DialogTitle>Summary of "{suggestedPropertySelectedForSummary.property.label}"</DialogTitle>
						</DialogHeader>
						<div className="py-4">
							<>
								<p>{suggestedPropertySelectedForSummary.property.summary}</p>
								<p className="mt-2 text-sm text-gray-700 font-semibold">
									Reason for suggestion:<br />
									<span className="font-normal">{suggestedPropertySelectedForSummary.property.reason}</span>
								</p>
								<p></p>
								{isSuggestionFromCurrentReply ? (
									<>
										<div className="flex items-center space-x-2 mt-4">
											<Checkbox
												id="optional-item"
												checked={suggestedPropertyAddAsOptional}
												onCheckedChange={(checked) => setSuggestedPropertyAddAsOptional(!!checked)}
											/>
											<Label htmlFor="optional-item">Add as OPTIONAL</Label>
										</div>
										{isDatatypeProperty(suggestedPropertySelectedForSummary.property) && (<div className="mt-4">
											<Label htmlFor="filter-expression">Filter expression</Label>
											<Input
												id="filter-expression"
												placeholder="e.g., {var} > 100"
												value={suggestedPropertyFilterExpression}
												onChange={(e) => setSuggestedPropertyFilterExpression(e.target.value)}
												disabled={suggestionIsSelected(suggestedPropertySelectedForSummary.property)}
											/>
										</div>)}
										{/* Add or Remove button */}
										{suggestionIsSelected(suggestedPropertySelectedForSummary.property) ? (
											<Button
												variant="destructive"
												className="mt-4"
												onClick={() => {
													handleRemoveSelectedProperty(suggestedPropertySelectedForSummary.property.iri);
												}}
											>
												Remove selection
											</Button>
										) : (
											<Button
												className="mt-4"
												onClick={handleAddSuggestedProperty}
											>
												Add item to my message
											</Button>
										)}
									</>
								) : (
									<p className="mt-4 text-sm text-gray-500 italic">
										Only items from the most recent reply can be added to your message.
									</p>
								)}
							</>
						</div>
					</DialogContent>
				</Dialog>
			)}
		</div>
	);
}

export default ConversationPageMock;