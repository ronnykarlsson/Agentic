using System.Threading.Tasks;

namespace Agentic.Clients.Ollama
{
    public interface IOllamaClient
    {
        Task<OllamaResponse> SendRequestAsync(OllamaRequest request);
    }
}