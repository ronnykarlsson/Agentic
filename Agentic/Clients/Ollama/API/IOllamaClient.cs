using System.Threading.Tasks;

namespace Agentic.Clients.Ollama.API
{
    public interface IOllamaClient
    {
        Task<float[]> GetEmbeddingsAsync(string input, string model);
        Task<OllamaResponse> SendRequestAsync(OllamaRequest request);
    }
}