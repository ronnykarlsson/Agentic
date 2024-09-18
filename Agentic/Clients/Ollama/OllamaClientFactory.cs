using Agentic.Chat;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Clients.Ollama
{
    public class OllamaClientFactory : IChatClientFactory, IEmbeddingClientFactory
    {
        private readonly IConfiguration _configuration;

        public OllamaClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => "Ollama";

        public IChatClient CreateChatClient(ClientSettings clientSettings)
        {
            return new OllamaChatClient(clientSettings);
        }

        public IEmbeddingClient CreateEmbeddingClient(ClientSettings clientSettings)
        {
            return new OllamaEmbeddingClient(clientSettings);
        }
    }
}
