namespace Agentic.Embeddings.Cache
{
    public interface IEmbeddingCache
    {
        bool TryGetEmbedding(string text, out float[] embedding);
        void SaveEmbedding(string text, float[] embedding);
    }
}