using Agentic;
using Agentic.Agents;
using Agentic.Profiles;
using Captain.Web.Client.Models;

namespace Captain.Web.Client.Services
{
    public class ChatService
    {
        private readonly IAgenticFactory _agenticFactory;
        private readonly AgentService _agentService;
        private readonly SettingsService _settingsService;
        private IChatAgent _chatAgent;

        public ChatService(IAgenticFactory agenticFactory, AgentService agentService, SettingsService settingsService)
        {
            _agenticFactory = agenticFactory;
            _agentService = agentService;
            _settingsService = settingsService;
        }

        public async Task InitializeAsync(string agentName, string? clientName)
        {
            await _agentService.InitializeAsync();
            var agent = await _agentService.GetAgentByNameAsync(agentName);

            ClientSettings? selectedClientSettings = null;

            if (!string.IsNullOrEmpty(clientName))
            {
                var uiClients = await _settingsService.LoadSettingsAsync();
                if (uiClients != null)
                {
                    UIClientSettings? selectedUIClient = uiClients.FirstOrDefault(c => c.ClientDisplayName == clientName);
                    if (selectedUIClient != null)
                    {
                        selectedClientSettings = selectedUIClient.ClientSettings;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Client with name '{clientName}' not found in settings.");
                    }
                }
            }

            _chatAgent = _agenticFactory.CreateFromString(agent.Profile, selectedClientSettings);
        }

        public async Task<string> ChatAsync(string input)
        {
            var response = await _chatAgent.ChatAsync(input).ConfigureAwait(false);
            return response;
        }
    }
}