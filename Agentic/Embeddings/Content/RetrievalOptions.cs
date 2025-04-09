namespace Agentic.Embeddings.Content
{
    public class RetrievalOptions
    {
        public int PrecedingChunks { get; set; } = 0;
        public int FollowingChunks { get; set; } = 0;
    }
}
