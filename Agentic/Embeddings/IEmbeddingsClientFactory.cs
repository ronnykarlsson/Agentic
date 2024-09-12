using Agentic.Profiles;

namespace Agentic.Embeddings
{
    public interface IEmbeddingsClientFactory
    {
        string Name { get; }
        IEmbeddingsClient CreateEmbeddingsClient();
        IEmbeddingsClient CreateEmbeddingsClient(ClientSettings clientSettings);
    }
}
