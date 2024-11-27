namespace Agentic.Profiles
{
    public class AgenticProfile
    {
        public ClientSettings Client { get; set; }
        public ClientSettings EmbeddingClient { get; set; }
        public CacheSettings Cache { get; set; } = new CacheSettings();
        public AgentDefinition Agent { get; set; }
    }
}
