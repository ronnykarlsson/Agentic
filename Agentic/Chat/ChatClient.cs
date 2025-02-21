﻿using Agentic.Agents;
using Agentic.Exceptions;
using Agentic.Tools;
using Agentic.Workspaces;
using Microsoft.ML.Tokenizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agentic.Chat
{
    public abstract class ChatClient<TRequest> : IChatClient
    {
        protected string SystemMessage;
        protected Toolbox Toolbox;
        protected IWorkspace[] Workspaces;

        TiktokenTokenizer _tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");

        public int MaxTokens { get; }

        protected abstract TRequest CreateRequest();
        protected abstract void AddRequestMessage(TRequest request, ChatMessage message);
        public abstract Task<ChatMessage> SendRequestAsync(TRequest request);

        private readonly string _workspaceDelimiter = "-";

        protected ChatClient(int maxTokens)
        {
            MaxTokens = maxTokens;
        }

        public virtual async Task<ChatMessage> SendAsync(ChatContext chatContext)
        {
            if (string.IsNullOrWhiteSpace(SystemMessage))
            {
                throw new InvalidOperationException($"System message must be set, call {nameof(SetSystemMessage)} first.");
            }

            var request = CreateRequest();

            var systemMessageString = Toolbox.CreateDefaultSystemMessage(SystemMessage);

            StringBuilder workspaceSystemMessageStringBuilder = null;
            if (Workspaces != null && Workspaces.Length > 0)
            {
                var executionContext = new AgentExecutionContext
                {
                    Messages = chatContext.Messages,
                    Workspaces = Workspaces
                };

                workspaceSystemMessageStringBuilder = new StringBuilder();
                foreach (var workspace in Workspaces)
                {
                    var workspacePrompt = workspace.GetPrompt(executionContext);
                    if (string.IsNullOrWhiteSpace(workspacePrompt)) continue;

                    workspaceSystemMessageStringBuilder.AppendLine(workspacePrompt);
                    workspaceSystemMessageStringBuilder.AppendLine(_workspaceDelimiter);
                }

                systemMessageString = $"{systemMessageString}{Environment.NewLine}{workspaceSystemMessageStringBuilder}";
            }

            var systemMessage = new ChatMessage(Role.System, systemMessageString);
            AddRequestMessage(request, systemMessage);

            var tokens = MaxTokens - CalculateTokens(systemMessage);
            AddRequestMessages(chatContext, request, tokens);

            var chatMessage = await SendRequestAsync(request).ConfigureAwait(false);

            return chatMessage;
        }

        public virtual async Task<ChatMessage> SendAsync(ChatContext chatContext, IList<ChatMessage> additionalMessages)
        {
            if (string.IsNullOrWhiteSpace(SystemMessage))
            {
                throw new InvalidOperationException($"System message must be set, call {nameof(SetSystemMessage)} first.");
            }

            // Calculate max tokens with system message tokens
            var toolsSystemMessage = Toolbox.CreateDefaultSystemMessage(SystemMessage);
            var systemMessage = new ChatMessage(Role.System, toolsSystemMessage);
            var tokens = MaxTokens - CalculateTokens(systemMessage);

            // Calculate max tokens including additional message
            var messageTokens = additionalMessages.Sum(CalculateTokens);
            tokens = tokens - messageTokens;
            if (tokens < 0) throw new ChatException("Message is too long");

            var request = CreateRequest();

            AddRequestMessage(request, systemMessage);
            AddRequestMessages(chatContext, request, tokens);

            foreach (var message in additionalMessages)
            {
                AddRequestMessage(request, message);
            }

            var chatMessage = await SendRequestAsync(request).ConfigureAwait(false);

            return chatMessage;
        }

        public void SetSystemMessage(string systemMessage)
        {
            if (string.IsNullOrWhiteSpace(systemMessage))
            {
                throw new ArgumentException($"'{nameof(systemMessage)}' cannot be null or whitespace.", nameof(systemMessage));
            }

            SystemMessage = systemMessage;
        }

        public string LimitMessageSize(string message, float percentMaxTokens)
        {
            var messageTokens = CalculateTokens(message);
            var maxTokens = (int)(MaxTokens * percentMaxTokens);
            if (messageTokens <= maxTokens) return message;

            var newMessageSize = ((float)maxTokens / (float)messageTokens) * message.Length;
            return message.Substring(0, (int) newMessageSize) + "...";
        }

        public void SetTools(Toolbox toolbox)
        {
            Toolbox = toolbox;
        }

        public void SetWorkspaces(IWorkspace[] workspaces)
        {
            Workspaces = workspaces;
        }

        private void AddRequestMessages(ChatContext context, TRequest request, int tokens)
        {
            var addMessages = new List<ChatMessage>();

            var tokensLeft = tokens;

            foreach (var chatMessage in context.Messages.GetReverseEnumerator())
            {
                tokensLeft -= CalculateTokens(chatMessage);
                if (tokensLeft <= 0) break;

                addMessages.Add(chatMessage);
            }

            for (int i = addMessages.Count - 1; i >= 0; i--)
            {
                var chatMessage = addMessages[i];
                AddRequestMessage(request, chatMessage);
            }
        }

        protected virtual int CalculateTokens(ChatMessage chatMesssage)
        {
            return CalculateTokens(chatMesssage.Role.ToString()) + CalculateTokens(chatMesssage.Content);
        }

        protected virtual int CalculateTokens(string message)
        {
            if (string.IsNullOrEmpty(message)) return 0;
            var tokens = _tokenizer.CountTokens(message);
            return tokens;
        }
    }
}
