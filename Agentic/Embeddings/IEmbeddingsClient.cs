using System.Threading.Tasks;

namespace Agentic.Embeddings
{
    public interface IEmbeddingsClient
    {
        Task<float[]> GetEmbeddingsAsync(string input);
    }
}
