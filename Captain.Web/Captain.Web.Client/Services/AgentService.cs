using Microsoft.JSInterop;

namespace Captain.Web.Client.Services
{
    public class AgentService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string AgentsKey = "agents";
        private const string DefaultAgentProfile =
@"agent:
  name: Assistant
  prompt: |
    You are a helpful assistant.
";

        public AgentService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
        }

        public async Task<List<Agent>> LoadAgentsAsync()
        {
            List<Agent> agents;
            try
            {
                var agentsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AgentsKey);
                agents = string.IsNullOrWhiteSpace(agentsJson) ? new List<Agent>() : System.Text.Json.JsonSerializer.Deserialize<List<Agent>>(agentsJson);
            }
            catch
            {
                agents = new List<Agent>();
            }

            var defaultAgents = GetDefaultAgents();
            foreach (var defaultAgent in defaultAgents.Reverse())
            {
                agents.RemoveAll(agent => agent.Name.Equals(defaultAgent.Name, StringComparison.InvariantCultureIgnoreCase));
                agents.Insert(0, defaultAgent);
            }

            return agents;
        }

        public async Task SaveAgentsAsync(List<Agent> agents)
        {
            var defaultAgentNames = GetDefaultAgents().Select(a => a.Name);
            var nonDefaultAgents = agents.Where(agent => !defaultAgentNames.Contains(agent.Name, StringComparer.InvariantCultureIgnoreCase));
            var saveAgents = new List<Agent>(nonDefaultAgents);

            var agentsJson = System.Text.Json.JsonSerializer.Serialize(saveAgents);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AgentsKey, agentsJson);
        }

        public async Task AddOrUpdateAgentAsync(Agent agent)
        {
            var agents = await LoadAgentsAsync();
            var existingAgent = agents.Find(a => a.Name == agent.Name);
            if (existingAgent != null)
            {
                existingAgent.Profile = agent.Profile;
            }
            else
            {
                agents.Add(agent);
            }
            await SaveAgentsAsync(agents);
        }

        public async Task<Agent> GetAgentByNameAsync(string name)
        {
            var agents = await LoadAgentsAsync();
            return agents.Find(a => a.Name == name);
        }

        public async Task<List<Agent>> ListAgentsAsync()
        {
            var agents = await LoadAgentsAsync();
            return agents;
        }

        private Agent[] GetDefaultAgents()
        {
            var defaultAgents = new Agent[]
                {
                    new Agent { Name = "Default", Profile = DefaultAgentProfile, IsDefault = true }
                };

            return defaultAgents;
        }
    }

    public class Agent
    {
        public string Name { get; set; }
        public string Profile { get; set; }
        public bool IsDefault { get; set; }
    }
}
