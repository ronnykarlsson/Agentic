using Agentic.Clients.Ollama;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;

namespace Agentic.Chat.Ollama
{
    public class OllamaChatClientFactory : IOllamaChatClientFactory
    {
        private readonly IOllamaClient _client;
        private readonly IConfiguration _configuration;

        public OllamaChatClientFactory(IOllamaClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public string Name => "Ollama";

        public IChatClient Create()
        {
            return new OllamaChatClient(_client, _configuration);
        }

        public IChatClient Create(ClientSettings clientSettings)
        {
            return new OllamaChatClient(clientSettings);
        }
    }
}
