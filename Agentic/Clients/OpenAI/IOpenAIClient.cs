using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI
{
    public interface IOpenAIClient
    {
        Task<OpenAIResponse> SendRequestAsync(OpenAIRequest request);
    }
}