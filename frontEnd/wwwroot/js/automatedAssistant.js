document.addEventListener('DOMContentLoaded', () => {
    const widget = document.getElementById('aa-widget');
    const closeBtn = document.getElementById('aa-close');
    const sendBtn = document.getElementById('aa-send');
    const inputField = document.getElementById('aa-input');
    const messagesArea = document.getElementById('aa-messages');
    
    // Check if user is logged in (using basic check if possible, else generic)
    const usernameElement = document.querySelector('.bg-white.border-b strong');
    let username = "Guest";
    if (usernameElement && usernameElement.innerText.trim() !== "" && !usernameElement.innerText.includes("Sign in")) {
        username = usernameElement.innerText.trim();
    }
    
    let isFirstOpen = true;

    // Define window method to open bot
    window.openBot = function() {
        if(widget) {
            widget.classList.add('active');
            
            if (isFirstOpen) {
                isFirstOpen = false;
                inputField.disabled = true;
                
                // Simulate typing delay for first messages
                setTimeout(() => {
                    addMessage("OK, no problem. Let me know what you need help with.", 'bot');
                }, 500);
                
                setTimeout(() => {
                    addMessage(`Hi, ${username}! I'm eBay's automated assistant. I only understand English, but can help with any issues you might have. I can also connect you with an agent.`, 'bot');
                }, 1500);
                
                setTimeout(() => {
                    addMessage("We may analyze this interaction to enhance the quality of our customer service, including by using artificial intelligence.", 'bot');
                    inputField.placeholder = "Type your message...";
                    inputField.disabled = false;
                    inputField.focus();
                }, 2500);
            }
        }
    };

    if (closeBtn) {
        closeBtn.addEventListener('click', () => {
            widget.classList.remove('active');
        });
    }

    // Input interaction
    if (inputField && sendBtn) {
        inputField.addEventListener('input', () => {
            if (inputField.value.trim() !== '') {
                sendBtn.classList.add('active');
            } else {
                sendBtn.classList.remove('active');
            }
        });

        inputField.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                handleSend();
            }
        });

        sendBtn.addEventListener('click', handleSend);
    }

    function handleSend() {
        const text = inputField.value.trim();
        if (text === '') return;
        
        // Add user message
        addMessage(text, 'user');
        inputField.value = '';
        sendBtn.classList.remove('active');
        inputField.focus();
        
        // Simulate bot reply
        setTimeout(() => {
            generateBotResponse(text);
        }, 1000);
    }

    function addMessage(text, sender) {
        const msgDiv = document.createElement('div');
        msgDiv.className = `aa-msg ${sender}`;
        msgDiv.innerText = text;
        
        messagesArea.appendChild(msgDiv);
        
        const labelDiv = document.createElement('div');
        labelDiv.className = 'aa-msg-label';
        labelDiv.innerText = sender === 'bot' ? 'Assistant' : 'You';
        
        if (sender === 'bot') {
            msgDiv.insertAdjacentElement('afterend', labelDiv);
        } else {
            labelDiv.style.alignSelf = 'flex-end';
            labelDiv.style.marginRight = '5px';
            msgDiv.insertAdjacentElement('afterend', labelDiv);
        }
        
        // Scroll to bottom
        messagesArea.scrollTop = messagesArea.scrollHeight;
    }

    function generateBotResponse(userText) {
        const lower = userText.toLowerCase();
        let reply = "I'm sorry, I don't understand that request. Would you like me to connect you with an agent?";
        
        if (lower.includes('status') || lower.includes('order') || lower.includes('track')) {
            reply = "You can track your order status in the 'My Purchases' section. Is there a specific order you need help with?";
        } else if (lower.includes('return') || lower.includes('refund')) {
            reply = "To start a return, go to your purchase history, find the item, and select 'Return this item'.";
        } else if (lower.includes('payment') || lower.includes('pay')) {
            reply = "If your payment is on hold, it usually clears within a few days after delivery. You can check your Seller Hub for details.";
        } else if (lower.includes('hello') || lower.includes('hi')) {
            reply = `Hello again! How can I assist you today?`;
        }
        
        addMessage(reply, 'bot');
    }
});
