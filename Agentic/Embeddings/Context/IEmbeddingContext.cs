using Agentic.Embeddings.Store;

namespace Agentic.Embeddings.Context
{
    public interface IEmbeddingContext
    {
        IEmbeddingService Service { get; }
        IEmbeddingStore Store { get; }
    }
}
