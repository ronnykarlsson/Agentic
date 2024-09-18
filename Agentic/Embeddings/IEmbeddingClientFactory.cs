using Agentic.Profiles;

namespace Agentic.Embeddings
{
    public interface IEmbeddingClientFactory
    {
        string Name { get; }
        IEmbeddingClient CreateEmbeddingClient(ClientSettings clientSettings);
    }
}
