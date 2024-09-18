using Agentic.Embeddings.Store;

namespace Agentic.Embeddings.Context
{
    public interface IEmbeddingContext
    {
        IEmbeddingClient Client { get; }
        IEmbeddingStore Store { get; }
    }
}
