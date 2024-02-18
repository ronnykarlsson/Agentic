﻿using AutoSharp.Tools;
using System.Threading.Tasks;

namespace AutoSharp.Chat
{
    public interface IChatClient
    {
        void SetSystemMessage(string systemMessage);
        void SetTools(ITool[] tools);
        Task<ChatMessage> SendAsync(ChatContext context);
        Task<ChatMessage> SendAsync(ChatContext context, ChatMessage message);
    }
}
