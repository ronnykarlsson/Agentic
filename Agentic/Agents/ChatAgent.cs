﻿using Agentic.Chat;
using Agentic.Tools;
using Agentic.Tools.Confirmation;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Agentic.Agents
{
    public class ChatAgent : IChatAgent
    {
        /// <inheritdoc/>
        public event EventHandler<ChatResponseEventArgs> ChatResponse;

        /// <summary>
        /// Maximum number of non-tool responses before ending chat, which might occur if AI isn't understanding the instructions.
        /// </summary>
        public int MaxNonToolResponses { get; set; } = 3;

        /// <summary>
        /// Limit tool response to percentage of token limit
        /// </summary>
        public float LimitToolResponseSize { get; set; } = 0.6f;

        private readonly IChatClient _client;
        private readonly IToolConfirmation _toolConfirmation;

        private ChatContext _chatContext;
        private Toolbox _toolbox;

        public ChatAgent(IChatClient client)
        {
            _client = client;
            _chatContext = new ChatContext();
        }

        public ChatAgent(IChatClient client, IToolConfirmation toolConfirmation)
        {
            _client = client;
            _toolConfirmation = toolConfirmation;
            _chatContext = new ChatContext();
        }

        /// <inheritdoc/>
        public void Initialize(string systemMessage, Toolbox toolbox)
        {
            _client.SetSystemMessage(systemMessage);
            _toolbox = toolbox;
            _client.SetTools(toolbox);
        }

        /// <inheritdoc/>
        public async Task<string> ChatAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _chatContext.AddMessage(Role.User, message);
            }

            var nonToolResponses = 0;
            ChatMessage response = null;
            ChatMessage previousResponse = null;
            while (true)
            {
                if (nonToolResponses >= MaxNonToolResponses) break;

                // Chat repeatedly for tool usage until done
                previousResponse = response;
                response = await _client.SendAsync(_chatContext).ConfigureAwait(false);

                var isEnded = response.Content.Contains(_toolbox.ChatEndString) || response.Content.EndsWith("?");
                response.Content = Regex.Replace(response.Content, $"['\"]?{_toolbox.ChatEndString}['\"]?", "");

                var tool = _toolbox.ParseTools(response.Content).FirstOrDefault();

                if (tool != null)
                {
                    var toolInvocationMessage = _toolbox.GetToolInvocationMessage(tool);
                    response.Content = toolInvocationMessage;
                }

                if (!string.IsNullOrWhiteSpace(response.Content))
                {
                    _chatContext.AddMessage(Role.Assistant, response.Content);
                    OnChatResponse(new ChatResponseEventArgs
                    {
                        Response = response.Content
                    });
                }

                if (tool != null)
                {
                    if (_toolConfirmation != null && tool.RequireConfirmation && !_toolConfirmation.Confirm(tool)) break;

                    var toolResponse = tool.Invoke();
                    toolResponse = StripAnsiColorCodes(toolResponse);
                    toolResponse = _client.LimitMessageSize(toolResponse, LimitToolResponseSize);
                    _chatContext.AddMessage(Role.User, toolResponse);
                    OnChatResponse(new ChatResponseEventArgs
                    {
                        Response = toolResponse,
                        IsTool = true
                    });

                    // Reset non-tool response counter when using tools
                    nonToolResponses = 0;
                }
                else
                {
                    nonToolResponses++;
                }

                if (tool == null && isEnded) break;
                if (previousResponse != null && response.Content == previousResponse.Content) break;
            }

            return response.Content;
        }

        /// <inheritdoc/>
        public ChatContext GetContext()
        {
            return _chatContext;
        }

        /// <inheritdoc/>
        public void SetContext(ChatContext context)
        {
            _chatContext = context;
        }

        private string StripAnsiColorCodes(string text)
        {
            // ANSI color code pattern
            var ansiCodePattern = @"\x1B\[[0-9;]*m";
            return Regex.Replace(text, ansiCodePattern, string.Empty);
        }

        private void OnChatResponse(ChatResponseEventArgs eventArgs)
        {
            ChatResponse?.Invoke(this, eventArgs);
        }
    }
}
