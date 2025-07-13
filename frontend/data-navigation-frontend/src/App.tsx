import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import ConversationPage from "@/pages/ConversationPage";
import ConversationManagementPage from "@/pages/ConversationManagementPage";
import HomePage from "./pages/HomePage";

function App() {
	console.log("function App() is running.");
	const handleOpenMostRecentConversation = () => {
		const LAST_CHOSEN_CONVERSATION_ID_STRING = "lastChosenConversationId";
		const lastChosenConversationId = sessionStorage.getItem(LAST_CHOSEN_CONVERSATION_ID_STRING);
		console.log(`${LAST_CHOSEN_CONVERSATION_ID_STRING}: ${lastChosenConversationId}`);
		const mostRecentConversationPath = lastChosenConversationId // Path for the "Most recent conversation" link
		? `/conversation/${lastChosenConversationId}`
		: '/manage-conversations?noConversationSelected=true';
		return mostRecentConversationPath;
	}

	return (
		<Router>
			<div className="flex flex-col h-screen">
				<header className="flex items-center justify-between p-4 border-b">
					<h1 className="text-xl font-bold">Data specification helper</h1>
					<nav>
						<ul className="flex space-x-4">
							<li>
								<Button asChild variant="ghost">
									<Link to="/">Home</Link>
								</Button>
							</li>
							<li>
								<Button asChild variant="ghost">
									<Link to="/manage-conversations">Manage conversations</Link>
								</Button>
							</li>
							<li>
								<Button asChild variant="ghost">
									<Link to={handleOpenMostRecentConversation()}>Most recent conversation</Link>
								</Button>
							</li>
						</ul>
					</nav>
				</header>
				<main className="flex-1 overflow-auto">
					<Routes>
						<Route path="/" element={<HomePage/>}/>
						<Route path="/conversation/:conversationId" element={<ConversationPage/>}/>
						<Route path="/manage-conversations" element={<ConversationManagementPage/>}/>
						<Route path="/manage-conversations/new" element={<ConversationManagementPage/>}/>
					</Routes>
				</main>
			</div>
		</Router>
	);
}

export default App;