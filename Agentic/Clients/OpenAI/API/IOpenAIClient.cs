using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI.API
{
    public interface IOpenAIClient
    {
        Task<OpenAIResponse> SendRequestAsync(OpenAIRequest request);
        Task<OpenAIEmbeddingResponse> SendEmbeddingsRequestAsync(OpenAIEmbeddingRequest request);
    }
}