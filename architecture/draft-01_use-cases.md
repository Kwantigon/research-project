# Use cases

Designing the back end architecture.

Thinking in terms of use cases.

### The user sends the first message of the conversation

User sends a message.

Front end POSTs the message to the back end.

Request router passes the request data to the correct handler.

Handler finds the conversation based on its ID in the request.
Handler adds the message to the conversation.

Handler needs to somehow figure out the state of the conversation. That this is the first message in the conversation and based on that, some methods must be called.

Handler calls the prompt constructor, passes the user's message and asks for a prompt.

Handler calls the LLM connector and passes the constructed prompt to send it to the LLM.

The LLM connector returns the answer from the LLM to the handler.</br>
- The answer should have a JSON structure.

Handler calls the response processor to parse the LLM's response.</br>
- If the response processor cannot parse the response, it likely means the LLM returned it in an incorrect format.

The response processor returns and object that contains the parsed entities that match entities from the data specification.

Handler uses the parsed entities to construct a subset of the data specification. And to mark, which words to highlight to the user etc.