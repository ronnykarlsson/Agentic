using Agentic.Embeddings.Cache;
using System;

namespace Agentic.Embeddings
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IEmbeddingClient _embeddingClient;
        private readonly IEmbeddingCache _embeddingCache;

        public EmbeddingService(IEmbeddingClient embeddingClient, IEmbeddingCache embeddingCache)
        {
            _embeddingClient = embeddingClient ?? throw new ArgumentNullException(nameof(embeddingClient));
            _embeddingCache = embeddingCache ?? throw new ArgumentNullException(nameof(embeddingCache));
        }

        public float[] GetEmbedding(string text)
        {
            if (_embeddingCache.TryGetEmbedding(text, out var embedding))
            {
                return embedding;
            }

            embedding = _embeddingClient.GetEmbeddingsAsync(text).GetAwaiter().GetResult();
            _embeddingCache.SaveEmbedding(text, embedding);
            return embedding;
        }
    }
}
