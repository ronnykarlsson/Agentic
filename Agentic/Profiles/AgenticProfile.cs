namespace Agentic.Profiles
{
    public class AgenticProfile
    {
        public ClientSettings Client { get; set; }
        public ClientSettings EmbeddingClient { get; set; }
        public AgentDefinition Agent { get; set; }
    }
}
