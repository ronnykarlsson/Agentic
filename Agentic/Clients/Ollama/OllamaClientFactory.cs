using Agentic.Chat;
using Agentic.Clients.Ollama.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Clients.Ollama
{
    public class OllamaClientFactory : IChatClientFactory, IEmbeddingsClientFactory
    {
        private readonly IOllamaClient _client;
        private readonly IConfiguration _configuration;

        public OllamaClientFactory(IOllamaClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public string Name => "Ollama";

        public IChatClient CreateChatClient()
        {
            return new OllamaChatClient(_client, _configuration);
        }

        public IChatClient CreateChatClient(ClientSettings clientSettings)
        {
            return new OllamaChatClient(clientSettings);
        }

        public IEmbeddingsClient CreateEmbeddingsClient()
        {
            return new OllamaEmbeddingsClient(_client, _configuration);
        }

        public IEmbeddingsClient CreateEmbeddingsClient(ClientSettings clientSettings)
        {
            return new OllamaEmbeddingsClient(clientSettings);
        }
    }
}
