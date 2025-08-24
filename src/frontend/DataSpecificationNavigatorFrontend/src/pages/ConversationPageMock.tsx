import { useState, useEffect, useRef } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Skeleton } from '@/components/ui/skeleton';
import { useParams } from "react-router-dom";
import { Send } from "lucide-react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Popover, PopoverTrigger, PopoverContent } from "@/components/ui/popover";

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
	suggestedFirstMessage: string;
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
	itemExpanded: string;
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
	domainLabel: string;
	range: string;
	rangeLabel: string;
}

export interface SubstructureObjectProperty {
	iri: string;
	label: string;
	domain: string;
	domainLabel: string;
	range: string;
	rangeLabel: string;
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
	// Split mapped items into anchored (have valid indices) vs unanchored
	const anchoredItems = mappedItems.filter(
		(item) =>
			typeof item.startIndex === "number" &&
			typeof item.endIndex === "number" &&
			item.startIndex >= 0 &&
			item.startIndex < item.endIndex &&
			text.substring(item.startIndex, item.endIndex) === item.mappedPhrase
	);

	const unanchoredItems = mappedItems.filter(
		(item) => !anchoredItems.includes(item)
	);

	// sort spans by start index (just in case backend sends unordered)
	const sortedByStartPositions = anchoredItems.sort((a, b) => a.startIndex - b.startIndex);

	const anchoredElements: React.ReactNode[] = [];
	let lastIndex = 0;

	sortedByStartPositions.forEach((item, i) => {
		// Add the text before this item.
		if (item.startIndex > lastIndex) {
			anchoredElements.push(<span key={`text-${i}`}>{text.slice(lastIndex, item.startIndex)}</span>);
		}

		// Add the clickable words.
		anchoredElements.push(
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

	// Add any trailing text after the last item.
	if (lastIndex < text.length) {
		anchoredElements.push(<span key="text-end">{text.slice(lastIndex)}</span>);
	}

	//return <>{elements}</>;
	return (
		<div>
			<p className="leading-relaxed">{anchoredElements}</p>

			{unanchoredItems.length > 0 && (
				<div className="mt-2">
					<Popover>
						<PopoverTrigger asChild>
							<Button variant="link" size="sm" className="text-xs p-0 h-auto">
								+{unanchoredItems.length} {unanchoredItems.length > 1 ? "items" : "item"} not directly mapped to any phrases
							</Button>
						</PopoverTrigger>
						<PopoverContent className="w-64" side="top" align="start">
							<p className="text-sm font-semibold mb-2">Referenced items</p>
							<ul className="space-y-1">
								{unanchoredItems.map((item) => (
									<li key={item.iri}>
										<Button
											variant="link"
											className="p-0 h-auto text-sm text-blue-600 underline cursor-pointer"
											onClick={(e) => {
												e.preventDefault();
												onMappedItemClick(item);
											}}
										>
											{item.label}
										</Button>
									</li>
								))}
							</ul>
						</PopoverContent>
					</Popover>
				</div>
			)}
		</div>
	);
}

const mockMessages: (WelcomeMessage | UserMessage | ReplyMessage)[] = [
	{
		id: "welcome-1",
		type: "WelcomeMessage",
		text: "Welcome to the Energy Knowledge Chatbot!",
		timestamp: new Date().toISOString(),
		dataSpecificationSummary: "This ontology describes renewable energy concepts, technologies, and related policies.",
		suggestedFirstMessage: "Ask about solar energy, wind power, or government subsidies."
	},
	{
		id: "user-1",
		type: "UserMessage",
		text: "Tell me about solar energy.",
		timestamp: new Date().toISOString()
	},
	{
		id: "reply-1",
		type: "ReplyMessage",
		text: "Here are some things related to solar energy:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#SolarEnergy",
				label: "Solar Energy",
				summary: "Energy derived from sunlight using panels or mirrors.",
				mappedPhrase: "solar energy",
				startIndex: 14,
				endIndex: 26,
			}
		],
		sparqlQuery: "SELECT ?panel WHERE { ?panel a :SolarPanel }",
		suggestions: {
			directConnections: [
				{
					itemExpanded: "SolarEnergy",
					suggestions: [
						{
							iri: "http://example.org/ontology#SolarPanel",
							label: "Solar Panel",
							connection: "-> hasPanel -> Solar Panel",
							reason: "Solar panels are the main component of solar energy systems.",
							summary: "Devices that convert sunlight into electricity.",
							type: "ObjectProperty"
						},
						{
							iri: "http://example.org/ontology#installedCapacity",
							label: "Installed Capacity",
							connection: "-> installedCapacity -> float",
							reason: "Capacity describes the amount of energy installed.",
							summary: "The rated power output capacity of a solar system.",
							type: "DatatypeProperty"
						}
					]
				}
			],
			indirectConnections: [
				{
					itemExpanded: "GovernmentSubsidy",
					suggestions: [
						{
							iri: "http://example.org/ontology#GovernmentSubsidy",
							label: "Government Subsidy",
							connection: "<- supports <- Government Subsidy",
							reason: "Government subsidies support solar energy adoption.",
							summary: "Financial support to encourage solar installations.",
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
		text: "And wind power?",
		timestamp: new Date().toISOString()
	},
	{
		id: "reply-2",
		type: "ReplyMessage",
		text: "Here are some things connected to wind power:",
		timestamp: new Date().toISOString(),
		mappedItems: [
			{
				iri: "http://example.org/ontology#WindPower",
				label: "Wind Power",
				summary: "Harnessing wind energy to produce electricity.",
				mappedPhrase: "wind power",
				startIndex: 17,
				endIndex: 27,
			}
		],
		sparqlQuery: "SELECT ?turbine WHERE { ?turbine a :WindTurbine }",
		suggestions: {
			directConnections: [
				{
					itemExpanded: "WindPower",
					suggestions: [
						{
							iri: "http://example.org/ontology#WindTurbine",
							label: "Wind Turbine",
							connection: "-> hasTurbine -> Wind Turbine",
							reason: "Wind turbines are essential components of wind power.",
							summary: "Towers that capture wind and generate electricity.",
							type: "ObjectProperty"
						},
						{
							iri: "http://example.org/ontology#averageWindSpeed",
							label: "Average Wind Speed",
							connection: "-> averageWindSpeed -> float",
							reason: "Wind power is strongly dependent on wind speed.",
							summary: "Average wind speed at the installation site.",
							type: "DatatypeProperty"
						}
					]
				}
			],
			indirectConnections: [
				{
					itemExpanded: "NoisePollution",
					suggestions: [
						{
							iri: "http://example.org/ontology#NoisePollution",
							label: "Noise Pollution",
							connection: "<- causedBy <- Noise Pollution",
							reason: "Wind turbines can generate local noise.",
							summary: "Environmental side effect of wind power.",
							type: "DatatypeProperty"
						}
					]
				}
			]
		}
	},
	{
		id: "user-3",
		type: "UserMessage",
		text: "What about hydropower?",
		timestamp: new Date().toISOString()
	},
	{
		id: "reply-3",
		type: "ReplyMessage",
		text: "Hydropower connects to the following items:",
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
			{
				iri: "http://example.org/ontology#ABC",
				label: "Mock item ABC",
				summary: "Summary for the mock item ABC.",
				mappedPhrase: "",
				startIndex: -1,
				endIndex: -1,
			},
			{
				iri: "http://example.org/ontology#DEF",
				label: "Mock item DEF",
				summary: "Summary for the mock item DEF.",
				mappedPhrase: "",
				startIndex: -1,
				endIndex: -1,
			}
		],
		sparqlQuery: "SELECT ?dam WHERE { ?dam a :HydroDam }",
		suggestions: {
			directConnections: [
				{
					itemExpanded: "Hydropower",
					suggestions: [
						{
							iri: "http://example.org/ontology#HydroDam",
							label: "Hydro Dam",
							connection: "-> hasDam -> Hydro Dam",
							reason: "Dams are used in hydropower systems.",
							summary: "Large structures that control water flow.",
							type: "ObjectProperty"
						}
					]
				}
			],
			indirectConnections: [
				{
					itemExpanded: "FishMigration",
					suggestions: [
						{
							iri: "http://example.org/ontology#FishMigration",
							label: "Fish Migration",
							connection: "<- affectedBy <- Fish Migration",
							reason: "Dams disrupt aquatic ecosystems.",
							summary: "Fish migration patterns disrupted by dams.",
							type: "DatatypeProperty"
						}
					]
				}
			]
		}
	}
];

const mockSubstructure: DataSpecificationSubstructure = {
	classItems: [
		{
			iri: "http://example.org/ontology#SolarEnergy",
			label: "Solar Energy",
			isSelectTarget: true,
			objectProperties: [
				{
					iri: "http://example.org/ontology#hasPanel",
					label: "hasPanel",
					domain: "Solar Energy",
					domainLabel: "Solar Energy",
					range: "Solar Panel",
					rangeLabel: "Solar Panel"
				}
			],
			datatypeProperties: [
				{
					iri: "http://example.org/ontology#installedCapacity",
					label: "installedCapacity",
					domain: "Solar Energy",
					domainLabel: "Solar Energy",
					range: "float",
					rangeLabel: "float"
				}
			]
		},
		{
			iri: "http://example.org/ontology#SolarPanel",
			label: "Solar Panel",
			isSelectTarget: false,
			objectProperties: [],
			datatypeProperties: []
		},
		{
			iri: "http://example.org/ontology#WindPower",
			label: "Wind Power",
			isSelectTarget: true,
			objectProperties: [
				{
					iri: "http://example.org/ontology#hasTurbine",
					label: "hasTurbine",
					domain: "Wind Power",
					domainLabel: "Wind Power",
					range: "Wind Turbine",
					rangeLabel: "Wind Turbine"
				}
			],
			datatypeProperties: [
				{
					iri: "http://example.org/ontology#averageWindSpeed",
					label: "averageWindSpeed",
					domain: "Wind Power",
					domainLabel: "Wind Power",
					range: "float",
					rangeLabel: "float"
				}
			]
		},
		{
			iri: "http://example.org/ontology#WindTurbine",
			label: "Wind Turbine",
			isSelectTarget: false,
			objectProperties: [],
			datatypeProperties: []
		},
		{
			iri: "http://example.org/ontology#Hydropower",
			label: "Hydropower",
			isSelectTarget: true,
			objectProperties: [
				{
					iri: "http://example.org/ontology#hasDam",
					label: "hasDam",
					domain: "Hydropower",
					domainLabel: "Hydropower",
					range: "Hydro Dam",
					rangeLabel: "Hydro Dam"
				}
			],
			datatypeProperties: []
		},
		{
			iri: "http://example.org/ontology#HydroDam",
			label: "Hydro Dam",
			isSelectTarget: false,
			objectProperties: [],
			datatypeProperties: []
		},
		{
			iri: "http://example.org/ontology#EnergyPolicy",
			label: "Energy Policy",
			isSelectTarget: true,
			objectProperties: [
				{
					iri: "http://example.org/ontology#provides",
					label: "provides",
					domain: "Energy Policy",
					domainLabel: "Energy Policy",
					range: "Subsidy",
					rangeLabel: "Subsidy"
				},
				{
					iri: "http://example.org/ontology#imposes",
					label: "imposes",
					domain: "Energy Policy",
					domainLabel: "Energy Policy",
					range: "Carbon Tax",
					rangeLabel: "Carbon Tax"
				}
			],
			datatypeProperties: []
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
	const [suggestedMessage, setSuggestedMessage] = useState<string | null>("This is a very long mock suggested message: aaaaaaaaaaaaaaaaaa bbbbbbbbbbbbbbbbbbbbbbbbb cccccccccccccccccc wwwwwwwwwwwwwwwwwwwwwww");
	const [isFetchingSuggestedMessage, setIsFetchingSuggestedMessage] = useState<boolean>(false);
	const [fetchSuggestedMessageError, setFetchSuggestedMessageError] = useState<string | null>(null);
	const [isSendingUserMessage, setIsSendingUserMessage] = useState<boolean>(false);
	const [sendUserMessageError, setSendUserMessageError] = useState<string | null>(null);

	const fetchMessages = async () => {
		setIsFetchingMessages(true);
		setFetchMessagesError(null);

		try {
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/messages`);
			if (!response.ok) {
				console.error("Fetch messages response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching messages.");
			}

			const data = await response.json();
			console.log(`Successfully fetched ${data.length} messages in the conversation.`);
			setMessages(data);

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

		} catch (error) {
			console.error(error);
			setFetchMessagesError("Failed to retrieve messages in the conversation.");
		} finally {
			setIsFetchingMessages(false);
		}
	};

	const fetchSubstructure = async () => {
		setIsFetchingSubstructure(true);
		setFetchSubstructureError(null);

		try {
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/data-specification-substructure`);
			if (!response.ok) {
				console.error("Fetch substructure response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching the conversation's data specification substructure.");
			}

			const data: DataSpecificationSubstructure = await response.json();
			console.log(`Successfully fetched the substructure containing ${data.classItems.length} class items.`);
			setDataSpecificationSubstructure(data);
		} catch (error) {
			console.error(error);
			setFetchSubstructureError('Failed to retrieve the mapped data specification items.');
		} finally {
			setIsFetchingSubstructure(false);
		}
	}

	useEffect(() => {
		//fetchMessages();
		//fetchSubstructure();

		setMessages(mockMessages);
		// Look for the most recent reply message.
		if (mockMessages.length > 1) {
			let replyMsgFound: boolean = false;
			let userMsgFound: boolean = false;
			for (let i = mockMessages.length - 1; i >= 0; i--) {
				if (mockMessages[i].type == "ReplyMessage" && !replyMsgFound) {
					setCurrentReplyMessage(mockMessages[i] as ReplyMessage);
					replyMsgFound = true;
				}
				if (mockMessages[i].type == "UserMessage" && !userMsgFound) {
					setCurrentUserMessage(mockMessages[i] as UserMessage);
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
		setDataSpecificationSubstructure(mockSubstructure);
	}, []);

	useEffect(() => {
		if (messagesEndRef.current) {
			messagesEndRef.current.scrollTop = messagesEndRef.current.scrollHeight;
		}
	}, [messages]);

	const handleSendUserMessage = async () => {
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

		fetchSubstructure();
	};

	const fetchSuggestedMessage = async () => {
		console.log("Selected properties:");
		selectedPropertiesForExpansion.forEach(p => console.log(`{ ${p.label}, ${p.isOptional}, ${p.filterExpression} }`));

		setFetchSuggestedMessageError(null);
		setIsFetchingSuggestedMessage(true);
		// Call back end API to get the suggested message.
		try {
			const response = await fetch(`${BACKEND_API_URL}/conversations/${conversationId}/user-selected-items`, {
				method: "PUT",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({
					userSelections: selectedPropertiesForExpansion.map(item => ({
						propertyIri: item.iri,
						isOptional: item.isOptional,
						filterExpression: item.filterExpression
					}))
				})
			});
			if (!response.ok) {
				console.error("Fetch suggested message response status: " + response.status);
				console.error(response.body);
				throw new Error("Error fetching the suggested message.");
			}
			const data = await response.json();
			setSuggestedMessage(data.suggestedMessage);
			setUserMessageInput(data.suggestedMessage);
		}
		catch (error) {
			console.log(error);
			setFetchSuggestedMessageError("Failed to generate a suggested message.");
		} finally {
			setIsFetchingSuggestedMessage(false);
			setSuggestedPropertyAddAsOptional(false);
			setSuggestedPropertyFilterExpression("");
		}
	}

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
			<div className={`flex flex-col ${showSubstructure ? "flex-[2]" : "flex-1"} overflow-y-auto`}>
				<div className="flex justify-end mb-2">
					<Button
						size="sm"
						className="bg-blue-600 hover:bg-blue-700 text-white rounded-full
												p-3 flex items-center justify-center shadow-md"
						onClick={() => setShowSubstructure(!showSubstructure)}
					>
						{showSubstructure ? <><ChevronRight size={16} />Hide mapped data specification items</> : <><ChevronLeft size={16} />Show mapped data specification items</>}
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
								<p className="text-red-700 font-medium">Failed to send your message.</p>
								<p className="text-sm text-red-600 mt-1">{sendUserMessageError}</p>
								{/*<Button
									variant="outline"
									size="sm"
									className="mt-2"
									onClick={handleSendUserMessage}
								>
									Try Again
								</Button>*/}
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
						<CardContent>
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
								handleSendUserMessage();
							}
						}}
						disabled={isSendingUserMessage}
					/>
					<Button
						onClick={handleSendUserMessage}
						disabled={isSendingUserMessage}
						className="bg-blue-600 hover:bg-blue-700 text-white rounded-full p-3 flex items-center justify-center 
    										shadow-md transition-colors duration-200 disabled:bg-gray-300 disabled:cursor-not-allowed"
					>
						<Send size={20} />
					</Button>
				</div>
			</div>

			{/* RIGHT: Substructure sidebar (toggleable) */}
			{showSubstructure && (
				<div className="w-75 border-l overflow-y-auto px-2 bg-gray-50">
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
									{classItem.objectProperties.length > 0 && (
										<div className="mt-2">
											<h4 className="text-sm font-medium text-gray-700">Object properties:</h4>
											<ul className="list-disc list-inside text-sm">
												{classItem.objectProperties.map((prop) => (
													<li key={prop.iri}>
														<span className="font-medium">{prop.label}</span>
														<span className="ml-1 text-xs text-gray-500">(→ {prop.rangeLabel})</span>
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
														<span className="ml-1 text-xs text-gray-500">({prop.rangeLabel})</span>
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
							{(mappedItemSelectedForSummary.mappedPhrase !== "") ? (
								<p className="mt-6 text-sm text-gray-700 font-semibold">
									{'('}Mapped from: <span className="font-normal">{mappedItemSelectedForSummary.mappedPhrase}</span>{')'}
								</p>
							) : (
								<p className="mt-6 text-sm text-gray-700 font-semibold">
									{'('}Not directly mapped to a phrase.{')'}
								</p>
							)
							}
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