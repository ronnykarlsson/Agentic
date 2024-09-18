using System.Threading.Tasks;

namespace Agentic.Embeddings
{
    public interface IEmbeddingClient
    {
        Task<float[]> GetEmbeddingsAsync(string input);
    }
}
