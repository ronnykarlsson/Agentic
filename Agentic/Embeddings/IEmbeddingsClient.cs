using System.Threading.Tasks;

namespace Agentic.Embeddings
{
    public interface IEmbeddingsClient
    {
        Task<float[]> SendAsync(string input);
    }
}
