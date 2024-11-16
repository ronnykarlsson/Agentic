namespace Agentic.Profiles
{
    public class AgentDefinition
    {
        public ClientSettings Client { get; set; }
        public string Name { get; set; }
        public string Prompt { get; set; }
        public WorkspaceDefinition[] Workspaces { get; set; }
        public ToolDefinition[] Tools { get; set; }
        public AgentDefinition[] Partners { get; set; }
        public FollowUpDefinition FollowUp { get; set; }
    }
}
