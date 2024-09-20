using Agentic.Embeddings.Store;

namespace Agentic.Embeddings.Context
{
    internal class EmbeddingContext : IEmbeddingContext
    {
        private static IEmbeddingService _service;
        public IEmbeddingService Service { get => _service; set => _service = value; }

        private static IEmbeddingStore _store;
        public IEmbeddingStore Store { get => _store; set => _store = value; }
    }
}
