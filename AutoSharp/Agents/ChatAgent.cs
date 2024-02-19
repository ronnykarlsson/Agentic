using AutoSharp.Chat;
using AutoSharp.Tools;
using AutoSharp.Tools.Confirmation;
using AutoSharp.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSharp.Agents
{
    public class ChatAgent : IChatAgent
    {
        public event EventHandler<string> ChatResponse;

        private readonly IChatClient _client;
        private readonly IToolConfirmation _toolConfirmation;

        private ChatContext _chatContext;
        private ITool[] _tools;

        public ChatAgent(IChatClient client, IToolConfirmation toolConfirmation)
        {
            _client = client;
            _toolConfirmation = toolConfirmation;
            _chatContext = new ChatContext();
        }

        public void Initialize(string systemMessage, params ITool[] tools)
        {
            SetSystemMessage(systemMessage);
            SetTools(tools);
        }

        public void SetSystemMessage(string systemMessage)
        {
            _client.SetSystemMessage(systemMessage);
        }

        public void SetTools(ITool[] tools)
        {
            _tools = tools;
            _client.SetTools(tools);
        }

        public async Task<string> ChatAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _chatContext.AddMessage(Role.User, $"Your task: {message}");
            }

            var nonToolResponses = 0;
            ChatMessage response = null;
            ChatMessage previousResponse = null;
            while (true)
            {
                if (nonToolResponses >= 5) break;

                // Chat repeatedly for tool usage until done
                previousResponse = response;
                response = await _client.SendAsync(_chatContext).ConfigureAwait(false);

                var isEnded = response.Content.Contains(ChatHelpers.ChatEndString) || response.Content.EndsWith("?");
                response.Content = ChatHelpers.RemoveChatEndString(response.Content);

                var toolInvocation = ChatHelpers.ParseTools(response.Content).FirstOrDefault();

                if (toolInvocation != null)
                {
                    var toolInvocationMessage = ChatHelpers.GetToolInvocationMessage(new[] { toolInvocation });
                    response.Content = toolInvocationMessage;
                }

                if (!string.IsNullOrWhiteSpace(response.Content))
                {
                    _chatContext.AddMessage(Role.Assistant, response.Content);
                    OnChatResponse(response.Content);
                }

                if (toolInvocation != null)
                {
                    var tool = _tools?.FirstOrDefault(t => t.Name == toolInvocation.Name);
                    if (tool == null)
                    {
                        _chatContext.AddMessage(Role.User, $"No tool named: {toolInvocation.Name}");
                    }
                    else
                    {
                        if (!_toolConfirmation.Confirm(toolInvocation)) break;
                        var toolResponse = tool.Invoke(toolInvocation.Parameter);
                        _chatContext.AddMessage(Role.User, toolResponse);
                        OnChatResponse(toolResponse);
                    }
                }
                else
                {
                    nonToolResponses++;
                }

                if (toolInvocation == null && isEnded) break;
                if (previousResponse != null && response.Content == previousResponse.Content) break;
            }

            return response.Content;
        }

        private void OnChatResponse(string response)
        {
            ChatResponse?.Invoke(this, response);
        }

        public ChatContext GetContext()
        {
            return _chatContext;
        }

        public void SetContext(ChatContext context)
        {
            _chatContext = context;
        }
    }
}
