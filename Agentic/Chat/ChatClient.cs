using Agentic.Exceptions;
using Agentic.Tools;
using Agentic.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agentic.Chat
{
    public abstract class ChatClient<TRequest> : IChatClient
    {
        protected string SystemMessage;
        protected ITool[] Tools;

        public int MaxTokens { get; }

        protected abstract TRequest CreateRequest();
        protected abstract void AddRequestMessage(TRequest request, ChatMessage message);
        public abstract Task<ChatMessage> SendRequestAsync(TRequest request);

        protected ChatClient(int maxTokens)
        {
            MaxTokens = maxTokens;
        }

        public virtual async Task<ChatMessage> SendAsync(ChatContext context)
        {
            if (string.IsNullOrWhiteSpace(SystemMessage))
            {
                throw new InvalidOperationException($"System message must be set, call {nameof(SetSystemMessage)} first.");
            }

            var request = CreateRequest();

            var toolsSystemMessage = ChatHelpers.CreateDefaultSystemMessage(SystemMessage, Tools);
            var systemMessage = new ChatMessage(Role.System, toolsSystemMessage);
            AddRequestMessage(request, systemMessage);

            var tokens = MaxTokens - CalculateTokens(systemMessage);
            AddRequestMessages(context, request, tokens);

            var chatMessage = await SendRequestAsync(request).ConfigureAwait(false);

            return chatMessage;
        }

        public virtual async Task<ChatMessage> SendAsync(ChatContext context, ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(SystemMessage))
            {
                throw new InvalidOperationException($"System message must be set, call {nameof(SetSystemMessage)} first.");
            }

            // Calculate max tokens with system message tokens
            var toolsSystemMessage = ChatHelpers.CreateDefaultSystemMessage(SystemMessage, Tools);
            var systemMessage = new ChatMessage(Role.System, toolsSystemMessage);
            var tokens = MaxTokens - CalculateTokens(systemMessage);

            // Calculate max tokens including user message
            var messageTokens = CalculateTokens(message);
            tokens = tokens - messageTokens;
            if (tokens < 0) throw new ChatException("Message is too long");

            var request = CreateRequest();

            AddRequestMessage(request, systemMessage);
            AddRequestMessages(context, request, tokens);
            AddRequestMessage(request, message);

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

        public void SetTools(ITool[] tools)
        {
            Tools = tools;
        }

        private void AddRequestMessages(ChatContext context, TRequest request, int tokens)
        {
            var addMessages = new List<ChatMessage>();

            var tokensLeft = tokens;
            for (int i = context.Messages.Count - 1; i >= 0; i--)
            {
                var chatMessage = context.Messages[i];

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
            return TokenHelpers.CalculateTokenCount(message);
        }
    }
}
