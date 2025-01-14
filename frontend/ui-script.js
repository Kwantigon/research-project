function addMessageToMessagesUl(message, messageFromWho) {
    let messageElement = document.createElement("li");
    messageElement.classList.add(messageFromWho);
    messageElement.innerHTML = "<p>" + message + "</p>";
    document.getElementById("messages-ul").appendChild(messageElement);
}

const messageInputSendBtnOnClick = () => {
    let messageInputTextarea = document.getElementById("message-input-textarea");
    let userMessage = messageInputTextarea.value;
    console.log(`The message-input-send-btn was clicked! Message is: \"${userMessage}\"`);
    if (!userMessage) return;

    let url = "https://localhost:7283/prompt?value=" + userMessage;
    fetch(url, {method: "POST"})
        .then(response => {
            if (response.ok) return response.text();
            else throw new Error("Failed to get an answer from the backend.");
        })
        .then(data => {
            addMessageToMessagesUl(data, "bot-message");
            // let responseMsgElement = document.createElement("li");
            // responseMsgElement.classList.add("bot-message");
            // responseMsgElement.innerHTML = "<p>" + data + "</p>";
            // document.getElementById("messages-ul").appendChild(responseMsgElement);
        })

    addMessageToMessagesUl(userMessage, "user-message");
    messageInputTextarea.value = "";
}

document.getElementById("message-input-send-btn").addEventListener("click", messageInputSendBtnOnClick);

const retrieveChatHistory = (chatId) => {
    let url = "https://localhost:7283/chat-history/" + chatId;
    console.log("Fetching from " + url);
    fetch(url, {method: "GET"})
        .then(response => response.json())
        .then(jsonArray => {
            let messagesUl = document.getElementById("messages-ul");
            messagesUl.innerHTML = "";
            for (let i = 0; i < jsonArray.length; i++) {
                let message = jsonArray[i];
                addMessageToMessagesUl(message.MessageText, message.MessageFrom);
            }
        });
}

