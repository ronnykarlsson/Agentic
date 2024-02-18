using AutoSharp.Clients.OpenAI;
using AutoSharp.Exceptions;
using AutoSharp.Tools;
using AutoSharp.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoSharp.Chat
{
    public class OpenAIChatClient : IOpenAIChatClient
    {
        private readonly IOpenAIClient _client;
        private string _systemMessage;
        private readonly string _model;
        private readonly int _maxTokens;
        private ITool[] _tools;

        public OpenAIChatClient(IOpenAIClient client, IConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = new OpenAIClient(configuration, null);
            _model = configuration["OpenAI:Model"] ?? "gpt-4-turbo-preview";
            _maxTokens = int.Parse(configuration["OpenAI:MaxTokens"] ?? "7000");
        }

        public OpenAIChatClient(IOpenAIClient client, string model, int maxTokens, params ITool[] tools)
        {
            _client = client;
            _model = model;
            _maxTokens = maxTokens;
            _tools = tools;
        }

        public void SetSystemMessage(string systemMessage)
        {
            if (string.IsNullOrWhiteSpace(systemMessage))
            {
                throw new ArgumentException($"'{nameof(systemMessage)}' cannot be null or whitespace.", nameof(systemMessage));
            }

            _systemMessage = systemMessage;
        }

        public void SetTools(ITool[] tools)
        {
            _tools = tools;
        }

        public async Task<ChatMessage> SendAsync(ChatContext context)
        {
            if (string.IsNullOrWhiteSpace(_systemMessage))
            {
                throw new InvalidOperationException($"System message must be set, call {nameof(SetSystemMessage)} first.");
            }

            var request = new OpenAIRequest(_model, _maxTokens);
            var toolsSystemMessage = ChatHelpers.CreateDefaultSystemMessage(_systemMessage, _tools);
            request.AddMessage(Role.System.ToString().ToLowerInvariant(), toolsSystemMessage);

            AddRequestMessages(context, request, _maxTokens);

            var chatMessage = await InternalSendAsync(request).ConfigureAwait(false);

            return chatMessage;
        }

        public async Task<ChatMessage> SendAsync(ChatContext context, ChatMessage message)
        {
            var request = new OpenAIRequest(_model, _maxTokens);
            var toolsSystemMessage = ChatHelpers.CreateDefaultSystemMessage(_systemMessage, _tools);
            request.AddMessage(Role.System.ToString().ToLowerInvariant(), toolsSystemMessage);

            var messageTokens = TokenHelpers.EstimateTokenCount(message.Content) + 1;
            var maxTokens = _maxTokens - messageTokens;
            if (maxTokens < 0) throw new ChatException("Message is too long");

            AddRequestMessages(context, request, maxTokens);
            request.AddMessage(Role.User.ToString().ToLowerInvariant(), message.Content);

            var chatMessage = await InternalSendAsync(request).ConfigureAwait(false);

            return chatMessage;
        }

        private void AddRequestMessages(ChatContext context, OpenAIRequest request, int maxTokens)
        {
            // Insert messages until max tokens is reached
            var messagePosition = request.Messages.Count;
            for (int i = context.Messages.Count - 1; i >= 0; i--)
            {
                var contextMessage = context.Messages[i];
                if (CalculateTokens(request) >= maxTokens) break;

                request.Messages.Insert(messagePosition, new OpenAIMessage(contextMessage.Role.ToString().ToLowerInvariant(), contextMessage.Content));
            }
        }

        private async Task<ChatMessage> InternalSendAsync(OpenAIRequest request)
        {
            OpenAIResponse response;
            try
            {
                response = await _client.SendRequestAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new ChatException("Failed to send request", e);
            }

            var responseMessage = response.Choices.First().Message.Content;
            var chatMessage = new ChatMessage(Role.Assistant, responseMessage);
            return chatMessage;
        }

        private int CalculateTokens(OpenAIRequest request)
        {
            var tokens = 0;
            foreach (var message in request.Messages)
            {
                tokens += 1; // Role token
                tokens += TokenHelpers.EstimateTokenCount(message.Content);
            }
            return tokens;
        }
    }
}
