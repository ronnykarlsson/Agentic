using Agentic.Clients.OpenAI.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIEmbeddingsClient : IEmbeddingsClient
    {
        private readonly IOpenAIClient _client;
        private readonly string _model;

        public OpenAIEmbeddingsClient(IOpenAIClient client, IConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = client;
            _model = configuration["OpenAI:EmbeddingsModel"] ?? "nomic-embed-text";
        }

        public OpenAIEmbeddingsClient(ClientSettings clientSettings)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));

            _client = new OpenAIClient(clientSettings.ApiKey, clientSettings.BaseUrl ?? "http://127.0.0.1:11434");
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
