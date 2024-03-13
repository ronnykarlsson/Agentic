using Agentic.Clients.OpenAI;
using Agentic.Exceptions;
using Agentic.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agentic.Chat.OpenAI
{
    public class OpenAIChatClient : ChatClient<OpenAIRequest>, IOpenAIChatClient
    {
        private readonly IOpenAIClient _client;
        private readonly string _model;

        public OpenAIChatClient(IOpenAIClient client, IConfiguration configuration)
            : base(int.Parse(configuration["OpenAI:MaxTokens"] ?? "7000"))
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = new OpenAIClient(configuration, null);
            _model = configuration["OpenAI:Model"] ?? "gpt-4-turbo-preview";
        }

        public OpenAIChatClient(IOpenAIClient client, string model, int maxTokens, Toolbox toolbox)
            : base(maxTokens)
        {
            _client = client;
            _model = model;
            Toolbox = toolbox;
        }

        public override async Task<ChatMessage> SendRequestAsync(OpenAIRequest request)
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

        protected override OpenAIRequest CreateRequest()
        {
            return new OpenAIRequest(_model, MaxTokens);
        }

        protected override void AddRequestMessage(OpenAIRequest request, ChatMessage message)
        {
            request.AddMessage(message.Role.ToString().ToLowerInvariant(), message.Content);
        }
    }
}
