import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import type {
  Message,
  WelcomeMessage,
  UserMessage,
  ReplyMessage,
  Suggestions,
  GroupedSuggestions,
  SelectedSuggestedItem,
  SuggestedProperty,
} from "./ConversationPageMock";

// ---------------- Message type-specific components ----------------

function WelcomeMessageCard({ message }: { message: WelcomeMessage }) {
  return (
    <Card className="mb-4 bg-green-50">
      <CardContent className="p-4 space-y-2">
        <p className="text-gray-800">{message.text}</p>
        <p className="text-sm text-gray-600">Summary: {message.dataSpecificationSummary}</p>
        <p className="italic text-gray-500">Suggested: {message.suggestedMessage}</p>
      </CardContent>
    </Card>
  );
}

function UserMessageCard({ message }: { message: UserMessage }) {
  return (
    <Card className="mb-4 bg-blue-50">
      <CardContent className="p-4">
        <p className="text-gray-800">{message.text}</p>
      </CardContent>
    </Card>
  );
}

interface ReplyMessageCardProps {
  message: ReplyMessage;
  selectedItemsForExpansion: SelectedSuggestedItem[];
  onSuggestedItemClick: (property: SuggestedProperty, replyMsg: ReplyMessage) => void;
}

function ReplyMessageCard({
  message,
  selectedItemsForExpansion,
  onSuggestedItemClick: onSuggestedItemClick,
}: ReplyMessageCardProps) {
  const renderSuggestions = (suggestions: Suggestions) => {
    const renderGroup = (group: GroupedSuggestions, keyPrefix: string) => (
      <div key={keyPrefix} className="ml-4 mt-2">
        <p className="text-sm font-medium italic">{group.domain}:</p>
        <ul className="list-disc list-inside ml-4 mt-1">
          {group.suggestions.map((item) => {
            const isSelected = selectedItemsForExpansion.some(
              (selected) => selected.iri === item.iri
            );
            return (
              <li key={item.iri} className="flex items-center space-x-1">
                <Button
                  variant="link"
                  onClick={() => onSuggestedItemClick(item, message)}
                  className="p-0 h-auto text-sm text-blue-600 underline cursor-pointer"
                >
                  {item.connection}
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
    );

    return (
      <div className="space-y-4">
        {suggestions.directConnections?.map((group, idx) =>
          renderGroup(group, `direct-${idx}`)
        )}
        {suggestions.indirectConnections?.map((group, idx) =>
          renderGroup(group, `indirect-${idx}`)
        )}
      </div>
    );
  };

  return (
    <Card className="mb-4 bg-gray-50">
      <CardContent className="p-4 space-y-4">
        {/* Reply text */}
        <p className="text-gray-800">{message.text}</p>

        {/* SPARQL query */}
        {message.sparqlQuery && (
          <div>
            <h4 className="text-sm font-semibold text-gray-600 mb-1">SPARQL query:</h4>
            <pre className="bg-gray-100 p-2 rounded text-sm overflow-x-auto">
              {message.sparqlQuery}
            </pre>
          </div>
        )}

        {/* Suggestions */}
        {message.suggestions && (
          <div>
            <h4 className="text-sm font-semibold text-gray-600 mb-2">You might also like:</h4>
            {renderSuggestions(message.suggestions)}
          </div>
        )}
      </CardContent>
    </Card>
  );
}

// ---------------- Main Messages List Component ----------------

interface MessagesListProps {
  messages: Message[];
  selectedItemsForExpansion: SelectedSuggestedItem[];
  onSuggestedItemClick: (property: SuggestedProperty, replyMsg: ReplyMessage) => void;
}

export default function MessagesList({
  messages,
  selectedItemsForExpansion,
  onSuggestedItemClick: onSuggestedItemClick,
}: MessagesListProps) {
  return (
    <div className="space-y-2">
      {messages.map((msg) => {
        switch (msg.type) {
          case "WelcomeMessage":
            return <WelcomeMessageCard key={msg.id} message={msg as WelcomeMessage} />;
          case "UserMessage":
            return <UserMessageCard key={msg.id} message={msg as UserMessage} />;
          case "ReplyMessage":
            return (
              <ReplyMessageCard
                key={msg.id}
                message={msg as ReplyMessage}
                selectedItemsForExpansion={selectedItemsForExpansion}
                onSuggestedItemClick={onSuggestedItemClick}
              />
            );
          default:
            return null;
        }
      })}
    </div>
  );
}
