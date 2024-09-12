using Agentic.Chat;
using Agentic.Clients.OpenAI.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIClientFactory : IChatClientFactory, IEmbeddingsClientFactory
    {
        private readonly IOpenAIClient _client;
        private readonly IConfiguration _configuration;

        public OpenAIClientFactory(IOpenAIClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public string Name => "OpenAI";

        public IChatClient CreateChatClient()
        {
            return new OpenAIChatClient(_client, _configuration);
        }

        public IChatClient CreateChatClient(ClientSettings clientSettings)
        {
            return new OpenAIChatClient(_configuration, clientSettings);
        }

        public IEmbeddingsClient CreateEmbeddingsClient()
        {
            return new OpenAIEmbeddingsClient(_client, _configuration);
        }

        public IEmbeddingsClient CreateEmbeddingsClient(ClientSettings clientSettings)
        {
            return new OpenAIEmbeddingsClient(clientSettings);
        }
    }
}
