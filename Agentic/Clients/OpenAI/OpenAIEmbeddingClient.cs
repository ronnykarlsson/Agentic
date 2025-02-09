using Agentic.Clients.OpenAI.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIEmbeddingClient : IEmbeddingClient
    {
        private readonly IOpenAIClient _client;
        private readonly string _model;

        public OpenAIEmbeddingClient(IOpenAIClient client, IConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = client;
            _model = configuration["OpenAI:EmbeddingsModel"] ?? "nomic-embed-text";
        }

        public OpenAIEmbeddingClient(ClientSettings clientSettings)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));

            _client = new OpenAIClient(clientSettings.ApiKey, clientSettings.Url ?? "https://api.openai.com/v1");
            _model = clientSettings.Model;
        }

        public async Task<float[]> GetEmbeddingsAsync(string input)
        {
            var response = await _client.SendEmbeddingsRequestAsync(new OpenAIEmbeddingRequest
            {
                Model = _model,
                Input = input
            });
            return response.Data.First().Embedding;
        }
    }
}
