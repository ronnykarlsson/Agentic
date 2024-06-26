using Agentic.Agents;
using System;
using System.Linq;
using System.Collections.Generic;
using Agentic.Chat;
using Agentic.Tools;
using Agentic.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Agentic
{
    public class AgenticFactory : IAgenticFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProfileLoader _profileLoader;

        public AgenticFactory(IServiceProvider serviceProvider, IProfileLoader profileLoader)
        {
            _serviceProvider = serviceProvider;
            _profileLoader = profileLoader;
        }

        public IChatAgent Create(string path)
        {
            var profile = _profileLoader.LoadProfileFromFile(path);
            return Create(profile);
        }

        public IChatAgent Create(AgenticProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            var agentSettings = profile.Agent;
            var defaultClientSettings = profile.Client;

            var chatAgent = CreateAgent(agentSettings, defaultClientSettings);

            return chatAgent;
        }

        private ChatAgent CreateAgent(AgentDefinition agentSettings, ClientSettings defaultClientSettings)
        {
            var clientSettings = agentSettings.Client ?? defaultClientSettings;

            // Create client to use for the agent
            var chatClientFactories = CreateInstances<IChatClientFactory>();
            var chatClientFactory = chatClientFactories.FirstOrDefault(m => m.Name == clientSettings.Name);
            var chatClient = chatClientFactory.Create(clientSettings);

            // Create toolbox for the agent
            var toolbox = Toolbox.Empty;
            if (agentSettings.Tools != null && agentSettings.Tools.Length > 0)
            {
                toolbox = CreateTools(agentSettings.Tools);
            }

            var chatAgent = new ChatAgent(chatClient);
            chatAgent.Name = agentSettings.Name;

            // Create partner agents and assign chat tool
            if (agentSettings.Partners?.Length > 0)
            {
                var partners = new List<IChatAgent>();

                foreach (var partner in agentSettings.Partners)
                {
                    var partnerAgent = CreateAgent(partner, defaultClientSettings);

                    partnerAgent.ChatResponse += (sender, e) =>
                    {
                        chatAgent.OnChatResponse(new ChatResponseEventArgs
                        {
                            Response = e.Response,
                            IsTool = e.IsTool,
                            Agent = partnerAgent
                        });
                    };

                    partners.Add(partnerAgent);
                }

                var chatTool = new ChatTool(partners);
                toolbox.AddTool(chatTool);

                foreach (var partner in partners)
                {
                    // Add partners to each other's chat tool
                    var partnerChatTool = partner.Toolbox.Tools.FirstOrDefault(t => t is ChatTool) as ChatTool;
                    if (partnerChatTool == null)
                    {
                        partnerChatTool = new ChatTool(partners);
                    }
                    else
                    {
                        var ignorePartnerNames = partnerChatTool.Agents.Select(a => a.Name).ToArray();
                        var chatPartners = partners.Where(p => !ignorePartnerNames.Contains(p.Name)).ToArray();
                        var newChatPartners = chatPartners.Concat(partnerChatTool.Agents).ToArray();
                        partnerChatTool = new ChatTool(newChatPartners);
                    }

                    partner.Toolbox.AddTool(partnerChatTool);
                }
            }

            chatAgent.Initialize(agentSettings.Prompt, toolbox);

            return chatAgent;
        }

        private Toolbox CreateTools(ToolDefinition[] toolDefinitions)
        {
            Toolbox toolbox;
            var agentTools = new List<ITool>();

            foreach (var tool in toolDefinitions)
            {
                var agentTool = CreateInstances<ITool>(tool.Name);

                if (agentTool.Count == 0)
                    throw new InvalidOperationException($"Tool {tool.Name} not found");

                if (agentTool.Count > 1)
                    throw new InvalidOperationException($"Multiple tools found with name {tool.Name}");

                agentTools.Add(agentTool[0]);
            }

            toolbox = new Toolbox(agentTools.ToArray());
            return toolbox;
        }

        private IReadOnlyList<T> CreateInstances<T>(string nameFilter = null)
            where T : class
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var instanceTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => nameFilter == null || type.Name.ToLowerInvariant() == nameFilter.ToLowerInvariant())
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            var instances = new List<T>();
            foreach (var type in instanceTypes)
            {
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, type) as T;
                if (instance != null)
                {
                    instances.Add(instance);
                }
            }

            return instances;
        }
    }
}
