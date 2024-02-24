﻿using System.Collections.Generic;

namespace Agentic.Chat
{
    public class ChatContext
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public void AddMessage(Role role, string content)
        {
            Messages.Add(new ChatMessage(role, content));
        }
    }
}