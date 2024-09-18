using Agentic.Profiles;

namespace Agentic.Embeddings
{
    public interface IEmbeddingClientFactory
    {
        string Name { get; }
        IEmbeddingClient CreateEmbeddingClient();
        IEmbeddingClient CreateEmbeddingClient(ClientSettings clientSettings);
    }
}
