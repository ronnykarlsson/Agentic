using Agentic.Clients.OpenAI;
using System.Linq;
using System.Threading.Tasks;

namespace Agentic.Embeddings.OpenAI
{
    public class OpenAIEmbeddingsClient : IOpenAIEmbeddingsClient
    {
        private readonly IOpenAIClient _client;

        public OpenAIEmbeddingsClient(IOpenAIClient client)
        {
            _client = client;
        }

        public async Task<float[]> SendAsync(string input)
        {
            var response = await _client.SendEmbeddingsRequestAsync(new OpenAIEmbeddingRequest
            {
                Model = "text-embedding-3-small",
                Input = input
            });
            return response.Data.First().Embedding;
        }
    }
}
