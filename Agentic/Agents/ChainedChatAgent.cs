﻿using System;
using System.Threading.Tasks;

namespace Agentic.Agents
{
    public class ChainedChatAgent
    {
        public event EventHandler<string> ChatResponse;

        private readonly IChatAgent[] _chatAgents;

        public ChainedChatAgent(IChatAgent[] chatAgents)
        {
            _chatAgents = chatAgents;
            var context = _chatAgents[0].GetContext();

            foreach (var chatAgent in chatAgents)
            {
                chatAgent.ChatResponse += (sender, eventArgs) => ChatResponse?.Invoke(sender, eventArgs.Response);
                chatAgent.SetContext(context);
            }
        }

        public async Task<string> ChatAsync(string message)
        {
            string response = message;
            foreach (var chatAgent in _chatAgents)
            {
                response = await chatAgent.ChatAsync(response).ConfigureAwait(false);
            }

            return response;
        }
    }
}
