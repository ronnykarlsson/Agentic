using Agentic.Chat;
using Agentic.Clients.OpenAI.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIClientFactory : IChatClientFactory, IEmbeddingClientFactory
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

        public IEmbeddingClient CreateEmbeddingClient()
        {
            return new OpenAIEmbeddingClient(_client, _configuration);
        }

        public IEmbeddingClient CreateEmbeddingClient(ClientSettings clientSettings)
        {
            return new OpenAIEmbeddingClient(clientSettings);
        }
    }
}
