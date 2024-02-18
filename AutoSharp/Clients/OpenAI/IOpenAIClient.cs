using System.Threading.Tasks;

namespace AutoSharp.Clients.OpenAI
{
    public interface IOpenAIClient
    {
        Task<OpenAIResponse> SendRequestAsync(OpenAIRequest request);
    }
}