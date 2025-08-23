import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Skeleton } from '@/components/ui/skeleton';
import { useParams } from "react-router-dom";
import MessagesList from "./MessagesList";

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

export interface SelectedSuggestedProperty extends SuggestedProperty {
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
		dataSpecificationSummary:
			"This data specification covers renewable energy and related policies.",
		suggestedMessage: "Ask about wind power, solar energy, or subsidies."
	},
	{
		id: "user-1",
		type: "UserMessage",
		text: "What is solar energy?",
		timestamp: new Date().toISOString(),
	},
	{
		id: "reply-1",
		type: "ReplyMessage",
		text: "Here are some suggestions related to solar energy:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#SolarEnergy",
				label: "Solar Energy",
				summary: "Energy derived from sunlight using panels or mirrors.",
				mappedPhrase: "solar energy",
				startIndex: 8,
				endIndex: 20,
			},
		],
		sparqlQuery: "SELECT ?panel WHERE { ?panel a :SolarPanel }",
		suggestions: {
			directConnections: [
				{
					domain: "RenewableEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#SolarPanel",
							label: "Solar Panel",
							connection: "→ part of → Solar Energy",
							reason: "Solar panels are the main component.",
							summary: "Devices that convert sunlight into electricity.",
							type: "ObjectProperty",
						},
					],
				},
			],
			indirectConnections: [
				{
					domain: "EnergyPolicy",
					suggestions: [
						{
							iri: "http://example.org/ontology#TaxIncentive",
							label: "Tax Incentive",
							connection: "← encourages ← Solar Energy",
							reason: "Governments incentivize solar adoption.",
							summary: "Tax benefits provided for using solar power.",
							type: "DatatypeProperty",
						},
					],
				},
			],
		},
	},
	{
		id: "user-2",
		type: "UserMessage",
		text: "Tell me more about wind power.",
		timestamp: new Date().toISOString(),
	},
	{
		id: "reply-2",
		type: "ReplyMessage",
		text: "Wind power is widely used. Here are related suggestions:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#WindPower",
				label: "Wind Power",
				summary: "Harnessing wind energy to produce electricity.",
				mappedPhrase: "wind power",
				startIndex: 13,
				endIndex: 23,
			},
		],
		sparqlQuery: "SELECT ?turbine WHERE { ?turbine a :WindTurbine }",
		suggestions: {
			directConnections: [
				{
					domain: "RenewableEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#WindTurbine",
							label: "Wind Turbine",
							connection: "→ part of → Wind Power",
							reason: "Wind turbines are essential for wind power.",
							summary: "Towers that capture wind and generate electricity.",
							type: "ObjectProperty",
						},
					],
				},
			],
			indirectConnections: [
				{
					domain: "Environment",
					suggestions: [
						{
							iri: "http://example.org/ontology#NoisePollution",
							label: "Noise Pollution",
							connection: "← caused by ← Wind Power",
							reason: "Wind turbines can create local noise.",
							summary: "Noise produced by rotating blades.",
							type: "DatatypeProperty",
						},
					],
				},
			],
		},
	},
	{
		id: "user-3",
		type: "UserMessage",
		text: "How about hydropower?",
		timestamp: new Date().toISOString(),
	},
	{
		id: "reply-3",
		type: "ReplyMessage",
		text: "Hydropower suggestions are as follows:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#Hydropower",
				label: "Hydropower",
				summary: "Electricity produced from flowing water.",
				mappedPhrase: "hydropower",
				startIndex: 11,
				endIndex: 21,
			},
		],
		sparqlQuery: "SELECT ?dam WHERE { ?dam a :HydroDam }",
		suggestions: {
			directConnections: [
				{
					domain: "RenewableEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#HydroDam",
							label: "Hydro Dam",
							connection: "→ part of → Hydropower",
							reason: "Dams store water for electricity generation.",
							summary: "Large structures that control water flow.",
							type: "ObjectProperty",
						},
					],
				},
			],
			indirectConnections: [
				{
					domain: "Environment",
					suggestions: [
						{
							iri: "http://example.org/ontology#FishMigration",
							label: "Fish Migration",
							connection: "← affected by ← Hydropower",
							reason: "Dams impact aquatic ecosystems.",
							summary: "Fish migration patterns disrupted by dams.",
							type: "DatatypeProperty",
						},
					],
				},
			],
		},
	},
	{
		id: "user-4",
		type: "UserMessage",
		text: "Show me connections with energy policy.",
		timestamp: new Date().toISOString(),
	},
	{
		id: "reply-4",
		type: "ReplyMessage",
		text: "Here are some policy-related connections:",
		timestamp: new Date().toISOString(),
		mappedItems: [],
		sparqlQuery: "SELECT ?policy WHERE { ?policy a :EnergyPolicy }",
		suggestions: {
			directConnections: [
				{
					domain: "EnergyPolicy",
					suggestions: [
						{
							iri: "http://example.org/ontology#Subsidy",
							label: "Subsidy",
							connection: "→ supports → Renewable Energy",
							reason: "Policies often include subsidies.",
							summary: "Financial support by governments.",
							type: "DatatypeProperty",
						},
						{
							iri: "http://example.org/ontology#CarbonTax",
							label: "Carbon Tax",
							connection: "→ discourages → Fossil Fuels",
							reason: "Carbon taxes promote renewables indirectly.",
							summary: "A tax on fossil fuel emissions.",
							type: "DatatypeProperty",
						},
					],
				},
			],
			indirectConnections: [],
		},
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
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const [messages, setMessages] = useState<Message[]>([]);
	const [isFetchingMessages, setIsFetchingMessages] = useState<boolean>(true);
	const [fetchMessagesError, setFetchMessagesError] = useState<string | null>(null);

	const [dataSpecificationSubstructure, setDataSpecificationSubstructure] = useState<DataSpecificationSubstructure | null>(null);
	const [isFetchingSubstructure, setIsFetchingSubstructure] = useState<boolean>(false);
	const [fetchSubstructureError, setFetchSubstructureError] = useState<string | null>(null);
	const [showSubstructure, setShowSubstructure] = useState<boolean>(true);

	const [currentReplyMessage, setCurrentReplyMessage] = useState<ReplyMessage | null>(null);
	const [currentUserMessage, setCurrentUserMessage] = useState<UserMessage | null>(null);

	const [mappedItemSelectedForSummary, setMappedItemSelectedForSummary] = useState<MappedItem | null>(null);
	const [isMappedItemSummaryDialogOpen, setIsMappedItemSummaryDialogOpen] = useState<boolean>(false);

	const [suggestedPropertySelectedForSummary, setSuggestedPropertySelectedForSummary] = useState<{ property: SuggestedProperty, replyMsg: ReplyMessage } | null>(null);
	const [isSuggestedPropertySummaryDialogOpen, setIsSuggestedPropertySummaryDialogOpen] = useState<boolean>(false);
	const [suggestedPropertyAddAsOptional, setSuggestedPropertyAddAsOptional] = useState<boolean>(false);
	const [suggestedPropertyFilterExpression, setSuggestedPropertyFilterExpression] = useState<string>("");
	const [selectedPropertiesForExpansion, setSelectedPropertiesForExpansion] = useState<SelectedSuggestedProperty[]>([]);

	const [userMessageInput, setUserMessageInput] = useState<string>("");
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>(null);
	const [isFetchingSuggestedMessage, setIsFetchingSuggestedMessage] = useState<boolean>(false);
	const [fetchSuggestedMessageError, setFetchSuggestedMessageError] = useState<string | null>(null);
	const [isSendingUserMessage, setIsSendingUserMessage] = useState<boolean>(false);
	const [sendUserMessageError, setSendUserMessageError] = useState<string | null>(null);

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
		setFetchSubstructureError(null);
		await new Promise(f => setTimeout(f, 1000));
		setDataSpecificationSubstructure(mockSubstructure);
		setIsFetchingSubstructure(false);
	};

	useEffect(() => {
		fetchMessages();
		fetchSubstructure();
	}, []);

	useEffect(() => {
		if (messagesEndRef.current) {
			messagesEndRef.current.scrollTop = messagesEndRef.current.scrollHeight;
		}
	}, [messages]);

	const handleSendMessage = async () => {
		if (userMessageInput.trim() === "") return;

		// Add user's message to conversation
		const messageToSend = userMessageInput;
		const userMessage: UserMessage = {
			id: "UserMessage-IdPending", // ID is generated on the back end. Will be set later.
			text: messageToSend,
			timestamp: new Date().toLocaleString(),
			type: "UserMessage"
		};
		setMessages((prevMessages) => [
			...prevMessages, userMessage
		]);

		// Clear input and suggested message.
		setSuggestedMessage(null);
		setCurrentUserMessage(userMessage);

		// Send to backend.
		setIsSendingUserMessage(true);
		setSendUserMessageError(null);
		try {
			const requestBody = JSON.stringify(
				{
					textValue: userMessage.text
				});
			const postUserMsgResponse = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: requestBody
			});

			if (!postUserMsgResponse.ok) {
				console.error("POST response status: " + postUserMsgResponse.status);
				console.error(postUserMsgResponse.body);
				throw new Error("Failed to send the message to the server.");
			}
			const postUserMsgData = await postUserMsgResponse.json();
			console.log(`postUserMsgData: ${JSON.stringify(postUserMsgData)}`);

			userMessage.id = postUserMsgData.id; // Set the message ID that the back end generated.
			userMessage.replyMessageUri = postUserMsgData.replyMessageUri;

			// Do another fetch to get the reply to user's message.
			console.log(`Fetching a reply from ${BACKEND_API_URL}${userMessage.replyMessageUri}`);
			const getReplyMsgResponse = await fetch(`${BACKEND_API_URL}${userMessage.replyMessageUri}`);
			if (!getReplyMsgResponse.ok) {
				console.error("Fetch reply response status: " + getReplyMsgResponse.status);
				console.error(getReplyMsgResponse.body);
				throw new Error("Failed to get the reply message from the server.")
			}
			const replyMsgData: ReplyMessage = await getReplyMsgResponse.json();
			console.log(`getReplyMsgData: ${JSON.stringify(replyMsgData)}`);

			setMessages((prevMessages) => [
				...prevMessages, replyMsgData
			]);
			setCurrentReplyMessage(replyMsgData);

			setUserMessageInput("");
			setSelectedPropertiesForExpansion([]);
		} catch (error) {
			console.error(error);
			if (error instanceof Error)
				setSendUserMessageError(error.message);
			else
				setSendUserMessageError("There was some kind of error while sending the message.");
		} finally {
			setIsSendingUserMessage(false);
		}
	};

	const handleSuggestedPropertyClick = async (property: SuggestedProperty, replyMsg: ReplyMessage) => {
		setSuggestedPropertySelectedForSummary({ property, replyMsg });
		setIsSuggestedPropertySummaryDialogOpen(true);
	};

	const addSelectedProperty = (property: SuggestedProperty, isOptional: boolean, filterExpression: string) => {
		const newItem: SelectedSuggestedProperty = {
			...property,
			isOptional: isOptional,
			filterExpression: filterExpression
		};
		setSelectedPropertiesForExpansion([...selectedPropertiesForExpansion, newItem]);
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
	const removeSelectedProperty = (iri: string) => {
		const updatedSelectedItems = selectedPropertiesForExpansion.filter(item => item.iri !== iri);
		setSelectedPropertiesForExpansion(updatedSelectedItems);
	}

	const fetchSuggestedMessage = async () => {
		// Send the selectedItemsForExpansion array to backend.
		console.log("Selected properties:");
		selectedPropertiesForExpansion.forEach(p => console.log(`{ ${p.label}, ${p.isOptional}, ${p.filterExpression} }`));
		console.log("Fetching the suggested message.");
	}

	const handleCheckboxSuggestedPropertyToggle = (
		property: SuggestedProperty,
		isSelected: boolean
	) => {
		if (isSelected) {
			// Add with default 'optional' and 'filter' values.
			addSelectedProperty(property, false, "");
		} else {
			// Remove
			removeSelectedProperty(property.iri);
		}
	};

	const handleUpdateSuggestedItemOptions = (
		iri: string,
		options: { isOptional?: boolean; filterExpression?: string }
	) => {
		setSelectedPropertiesForExpansion((prev) =>
			prev.map((item) =>
				item.iri === iri ? { ...item, ...options } : item
			)
		);
	};

	const isSuggestionFromCurrentReply = (suggestedPropertySelectedForSummary?.replyMsg === currentReplyMessage);
	const suggestionIsSelected = (suggestedProperty: SuggestedProperty) => {
		return selectedPropertiesForExpansion.some(item => item.iri === suggestedProperty.iri);
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
					) : fetchMessagesError ? (
						<div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
							<strong className="font-bold">Error!</strong>
							<span className="block sm:inline"> {fetchMessagesError}</span>
						</div>
					) : (
						<MessagesList
							messages={messages}
							selectedItemsForExpansion={selectedPropertiesForExpansion}
							onSuggestedPropertyClick={handleSuggestedPropertyClick}
							onToggleSuggestedProperty={handleCheckboxSuggestedPropertyToggle}
							onUpdateSuggestedPropertyOptions={handleUpdateSuggestedItemOptions}
							onAddAllSelected={fetchSuggestedMessage}
							currentReplyMessageId={currentReplyMessage?.id ?? null}
						/>
					)}

					{/* Display a rotating circle while waiting for reply */}
					{isSendingUserMessage ? (
						<div className="flex justify-start">
							<Card className="bg-gray-100 max-w-2xl">
								<CardContent className="p-3">
									<div className="h-4 w-4 rounded-full border-2 border-gray-300 border-t-blue-500 animate-spin"></div>
									<div>Thinking....</div>
									<div>This might take a minute.</div>
								</CardContent>
							</Card>
						</div>
					) : sendUserMessageError && (
						<Card className="flex justify-start bg-red-50 border-red-200 max-w-2xl">
							<CardContent className="p-3">
								<p className="text-sm text-red-600 mt-1">{sendUserMessageError}</p>
								<Button
									variant="outline"
									size="sm"
									className="mt-2"
									onClick={handleSendMessage}
								>
									Try Again
								</Button>
							</CardContent>
						</Card>
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
				) : fetchSuggestedMessageError ? (
					<Card className="mt-4 p-3 bg-red-50 border-red-200">
						<CardContent>
							<p className="text-sm text-red-700 font-medium">Error: {fetchSuggestedMessageError}</p>
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
					<Card className="mt-4 bg-blue-50">
						<CardContent className="p-4">
							{isSendingUserMessage ? (
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
					<Button onClick={handleSendMessage} disabled={isSendingUserMessage}>SEND</Button>
				</div>
			</div>

			{/* RIGHT: Substructure sidebar (toggleable) */}
			{showSubstructure && (
				<div className="1/4 border rounded-md p-4 flex flex-col bg-gray-50">
					<h2 className="text-lg font-bold mb-2">Mapped data specification items</h2>
					{isFetchingSubstructure ? (
						<p>Loading items...</p>
					) : fetchSubstructureError ? (
						<p className="text-red-500">{fetchSubstructureError}</p>
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
								<p className="mt-2 mb-4 text-sm text-gray-700 font-semibold">
									Reason for suggestion:<br />
									<span className="font-normal">{suggestedPropertySelectedForSummary.property.reason}</span>
								</p>
								{isSuggestionFromCurrentReply ? (
									<>
										{/*<div className="flex items-center space-x-2">
											<Checkbox
												id="optional-item"
												checked={selectedPropertiesForExpansion.find(p => p.iri === suggestedPropertySelectedForSummary.property.iri)?.isOptional ?? suggestedPropertyAddAsOptional}
												onCheckedChange={(checked) => {
													const selected = selectedPropertiesForExpansion.find(p => p.iri === suggestedPropertySelectedForSummary.property.iri);
													if (selected) {
														selected.isOptional = !!checked;
													} else {
														setSuggestedPropertyAddAsOptional(!!checked);
													}
												}}
												className="w-6 h-6 border-2 border-gray-700 data-[state=checked]:bg-gray-700"
											/>
											<Label htmlFor="optional-item">Add as OPTIONAL</Label>
										</div>*/}
										{/*isDatatypeProperty(suggestedPropertySelectedForSummary.property) && (<div className="mt-4">
											<Label htmlFor="filter-expression">Filter expression</Label>
											<Input
												id="filter-expression"
												placeholder="e.g., {var} > 100"
												value={selectedPropertiesForExpansion.find(p => p.iri === suggestedPropertySelectedForSummary.property.iri)?.filterExpression ?? suggestedPropertyFilterExpression}
												onChange={(e) => {
													const selected = selectedPropertiesForExpansion.find(p => p.iri === suggestedPropertySelectedForSummary.property.iri);
													if (selected) {
														selected.filterExpression = e.target.value;
													} else {
														setSuggestedPropertyFilterExpression(e.target.value);
													}
												}}
											/>
										</div>)*/}
										{/* Add or Remove button */}
										{suggestionIsSelected(suggestedPropertySelectedForSummary.property) ? (
											<Button
												variant="destructive"
												className="mt-4"
												onClick={() => {
													removeSelectedProperty(suggestedPropertySelectedForSummary.property.iri);
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