using Agentic.Clients.OpenAI;
using Microsoft.Extensions.Configuration;

namespace Agentic.Chat
{
    public class OpenAIChatClientFactory : IOpenAIChatClientFactory
    {
        private readonly IOpenAIClient _client;
        private readonly IConfiguration _configuration;

        public OpenAIChatClientFactory(IOpenAIClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public IOpenAIChatClient Create()
        {
            return new OpenAIChatClient(_client, _configuration);
        }
    }
}
