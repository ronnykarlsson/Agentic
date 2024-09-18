using Agentic.Embeddings.Store;

namespace Agentic.Embeddings.Context
{
    internal class EmbeddingContext : IEmbeddingContext
    {
        private static IEmbeddingClient _client;
        public IEmbeddingClient Client { get => _client; set => _client = value; }

        private static IEmbeddingStore _store;
        public IEmbeddingStore Store { get => _store; set => _store = value; }
    }
}
