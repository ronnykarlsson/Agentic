using Agentic.Chat;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIClientFactory : IChatClientFactory, IEmbeddingClientFactory
    {
        private readonly IConfiguration _configuration;

        public OpenAIClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => "OpenAI";

        public IChatClient CreateChatClient(ClientSettings clientSettings)
        {
            return new OpenAIChatClient(_configuration, clientSettings);
        }

        public IEmbeddingClient CreateEmbeddingClient(ClientSettings clientSettings)
        {
            return new OpenAIEmbeddingClient(clientSettings);
        }
    }
}
