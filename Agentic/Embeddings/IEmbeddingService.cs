namespace Agentic.Embeddings
{
    public interface IEmbeddingService
    {
        float[] GetEmbedding(string text);
    }
}