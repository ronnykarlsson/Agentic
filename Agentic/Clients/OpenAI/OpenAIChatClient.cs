using Agentic.Chat;
using Agentic.Clients.OpenAI.API;
using Agentic.Exceptions;
using Agentic.Profiles;
using Agentic.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIChatClient : ChatClient<OpenAIRequest>
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

        public OpenAIChatClient(IConfiguration configuration, ClientSettings clientSettings)
            : base(clientSettings.Tokens)
        {
            var apiKey = clientSettings.ApiKey ?? configuration["OpenAI:ApiKey"];
            var baseUrl = clientSettings.BaseUrl ?? "https://api.openai.com/v1";

            _client = new OpenAIClient(apiKey, baseUrl);
            _model = clientSettings.Model;
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
