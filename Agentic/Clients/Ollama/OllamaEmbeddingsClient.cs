using Agentic.Clients.Ollama.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Agentic.Clients.Ollama
{
    public class OllamaEmbeddingsClient : IEmbeddingsClient
    {
        private readonly IOllamaClient _client;
        private readonly string _model;

        public OllamaEmbeddingsClient(IOllamaClient client, IConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = client;
            _model = configuration["Ollama:EmbeddingsModel"] ?? "nomic-embed-text";
        }

        public OllamaEmbeddingsClient(ClientSettings clientSettings)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));

            _client = new OllamaClient(clientSettings.ApiKey, clientSettings.BaseUrl ?? "http://127.0.0.1:11434");
            _model = clientSettings.Model;
        }

        public async Task<float[]> GetEmbeddingsAsync(string input)
        {
            var response = await _client.GetEmbeddingsAsync(input, _model).ConfigureAwait(false);
            return response;
        }
    }
}
