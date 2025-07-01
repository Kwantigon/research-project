import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Button } from '@/components/ui/button'; // Assuming shadcn/ui button
import ConversationPage from '@/pages/ConversationPage';
import ConversationManagementPage from '@/pages/ConversationManagementPage';

function App() {
  return (
    <Router>
      <div className="flex flex-col h-screen">
        <header className="flex items-center justify-between p-4 border-b">
          <h1 className="text-xl font-bold">Data specification helper</h1>
          <nav>
            <ul className="flex space-x-4">
              <li>
                <Button asChild variant="ghost">
                  <Link to="/">Chat</Link>
                </Button>
              </li>
              <li>
                <Button asChild variant="ghost">
                  <Link to="/manage-conversations">Manage Conversations</Link>
                </Button>
              </li>
            </ul>
          </nav>
        </header>
        <main className="flex-1 overflow-auto">
          <Routes>
            <Route path="/" element={<ConversationPage />} />
            <Route path="/manage-conversations" element={<ConversationManagementPage />} />
            {/* Potentially a route for selecting a Dataspecer package, if not handled purely by redirect */}
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;