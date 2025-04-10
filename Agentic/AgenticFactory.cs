﻿using Agentic.Agents;
using System;
using System.Linq;
using System.Collections.Generic;
using Agentic.Chat;
using Agentic.Tools;
using Agentic.Profiles;
using Microsoft.Extensions.DependencyInjection;
using Agentic.Workspaces;
using Agentic.Embeddings;
using Agentic.Embeddings.Cache;
using Agentic.Embeddings.Context;
using Agentic.Embeddings.Store;
using System.IO;
using Microsoft.Extensions.Configuration;
using Agentic.Utilities;
using Agentic.Chat.Middleware;

namespace Agentic
{
    public class AgenticFactory : IAgenticFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProfileLoader _profileLoader;
        private readonly IConfiguration _configuration;

        private string _profilePath;

        public AgenticFactory(IServiceProvider serviceProvider, IProfileLoader profileLoader, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _profileLoader = profileLoader;
            _configuration = configuration;
        }

        public IChatAgent CreateFromFile(string path)
        {
            _profilePath = FilePathResolver.ResolvePath(path);
            var profile = _profileLoader.LoadProfileFromFile(_profilePath);
            return Create(profile);
        }

        public IChatAgent CreateFromString(string yamlProfile)
        {
            var profile = _profileLoader.LoadProfileFromString(yamlProfile);
            return Create(profile);
        }

        public IChatAgent CreateFromString(string yamlProfile, ClientSettings clientSettings)
        {
            var profile = _profileLoader.LoadProfileFromString(yamlProfile);
            return Create(profile, clientSettings);
        }

        public IChatAgent Create(AgenticProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            var agentSettings = profile.Agent;
            var clientSettings = profile.Client ?? new ClientSettings();

            // Update client settings with configuration values
            UpdateClientSettingsFromConfiguration(clientSettings);

            var embeddingContext = new EmbeddingContext();

            var embeddingClientSettings = profile.EmbeddingClient;
            if (embeddingClientSettings != null)
            {
                UpdateClientSettingsFromConfiguration(embeddingClientSettings);

                var embeddingClientFactories = CreateInstances<IEmbeddingClientFactory>();
                var embeddingClientFactory = embeddingClientFactories.FirstOrDefault(m => m.Name == embeddingClientSettings.Name);
                if (embeddingClientFactory == null) throw new InvalidOperationException($"Embedding client {embeddingClientSettings.Name} not found");

                var embeddingClient = embeddingClientFactory.CreateEmbeddingClient(embeddingClientSettings);

                var cacheFolder = Path.Combine(profile.Cache.Folder, "embeddings");

                embeddingContext.Service = new EmbeddingService(embeddingClient, new FileEmbeddingCache(cacheFolder, $"{embeddingClientSettings.Name.ToLowerInvariant()}-{embeddingClientSettings.Model.ToLowerInvariant()}"));
                embeddingContext.Store = new EmbeddingStore();
            }

            var chatAgent = CreateAgent(agentSettings, clientSettings, null);

            return chatAgent;
        }

        public IChatAgent Create(AgenticProfile profile, ClientSettings clientSettings)
        {
            var agentSettings = profile.Agent;
            var chatAgent = CreateAgent(agentSettings, clientSettings, null);
            return chatAgent;
        }

        private void UpdateClientSettingsFromConfiguration(ClientSettings clientSettings)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));
            if (clientSettings.Name == null) throw new ArgumentException(nameof(clientSettings), $"{nameof(ClientSettings)}.{nameof(ClientSettings.Name)} missing");

            var configSection = _configuration.GetSection(clientSettings.Name);
            var agenticConfigSection = _configuration.GetSection("Agentic");
            var agenticClientConfigSection = agenticConfigSection.GetSection("Client");

            clientSettings.Url ??= configSection[nameof(clientSettings.Url)] ?? agenticClientConfigSection[nameof(clientSettings.Url)];
            clientSettings.ApiKey ??= configSection[nameof(clientSettings.ApiKey)] ?? agenticClientConfigSection[nameof(clientSettings.ApiKey)];
            clientSettings.Model ??= configSection[nameof(clientSettings.Model)] ?? agenticClientConfigSection[nameof(clientSettings.Model)];

            var configTokensString = configSection[nameof(clientSettings.Tokens)];
            if (configTokensString != null && int.TryParse(configTokensString, out var configurationTokens))
            {
                clientSettings.Tokens = configurationTokens;
            }
        }

        private ChatAgent CreateAgent(AgentDefinition agentSettings, ClientSettings defaultClientSettings, IWorkspace[] inputWorkspaces)
        {
            var clientSettings = agentSettings.Client ?? defaultClientSettings;

            UpdateClientSettingsFromConfiguration(clientSettings);

            // Create client to use for the agent
            var chatClientFactories = CreateInstances<IChatClientFactory>();
            var chatClientFactory = chatClientFactories.FirstOrDefault(m => m.Name == clientSettings.Name);
            if (chatClientFactory == null) throw new InvalidOperationException($"Chat client {clientSettings.Name} not found");

            var chatClient = chatClientFactory.CreateChatClient(clientSettings);

            IWorkspace[] agentWorkspaces = Array.Empty<IWorkspace>();
            // Create workspaces for the agent
            if (agentSettings.Workspaces != null && agentSettings.Workspaces.Length > 0)
            {
                agentWorkspaces = CreateWorkspaces(agentSettings.Workspaces);
            }

            var workspaces = (inputWorkspaces ?? Array.Empty<IWorkspace>()).Concat(agentWorkspaces).ToArray();
            
            // Create toolbox for the agent
            var toolbox = Toolbox.Empty;
            if (agentSettings.Tools != null && agentSettings.Tools.Length > 0)
            {
                toolbox = CreateTools(agentSettings.Tools);
            }

            var agentChatClient = chatClient;
            if (agentSettings.Refine != null)
            {
                var chatClientWrapper = new ChatClientWrapper(chatClient);
                chatClientWrapper.Use(new RefinementChatMiddleware(agentSettings.Refine));
                agentChatClient = chatClientWrapper;
            }

            var chatAgent = new ChatAgent(agentChatClient);
            chatAgent.Name = agentSettings.Name;

            // Create partner agents and assign chat tool
            if (agentSettings.Partners?.Length > 0)
            {
                var partners = new List<IChatAgent>();

                foreach (var partner in agentSettings.Partners)
                {
                    var partnerAgent = CreateAgent(partner, defaultClientSettings, workspaces);

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

            var followUpAgentFactory = agentSettings.FollowUp != null
                ? CreateFollowUpAgentFactory(defaultClientSettings, workspaces, chatAgent, agentSettings.FollowUp)
                : null;
            chatAgent.Initialize(agentSettings.Prompt, toolbox, workspaces, followUpAgentFactory);

            return chatAgent;
        }

        private Func<FollowUpAgentPrompt> CreateFollowUpAgentFactory(ClientSettings defaultClientSettings, IWorkspace[] inputWorkspaces, ChatAgent parent, FollowUpDefinition followUp)
        {
            return () =>
            {
                ChatAgent followUpAgent;
                if (followUp.Agent != null)
                {
                    followUpAgent = CreateAgent(followUp.Agent, defaultClientSettings, inputWorkspaces);

                    Func<FollowUpAgentPrompt> followUpFactory = null;
                    var nextFollowUp = followUp.Agent?.FollowUp;
                    if (nextFollowUp != null)
                    {
                        followUpFactory = CreateFollowUpAgentFactory(defaultClientSettings, inputWorkspaces, followUpAgent, nextFollowUp);
                    }

                    // Forward chat responses from follow-up agent to parent agent
                    followUpAgent.ChatResponse += (sender, e) =>
                    {
                        parent.OnChatResponse(e);
                    };

                    // Share context with both agents
                    var parentContext = parent.GetContext();
                    followUpAgent.SetContext(parentContext);
                }
                else
                {
                    // Follow up with same agent if no new agent is specified
                    followUpAgent = parent;
                }

                var response = new FollowUpAgentPrompt
                {
                    Agent = followUpAgent,
                    Prompt = followUp.Prompt
                };

                return response;
            };
        }

        private IWorkspace[] CreateWorkspaces(WorkspaceDefinition[] workspaceDefinitions)
        {
            var workspaces = new List<IWorkspace>();

            foreach (var workspace in workspaceDefinitions)
            {
                var workspaceType = CreateInstances<IWorkspace>(workspace.Type, "Workspace");

                if (workspaceType.Count == 0)
                    throw new InvalidOperationException($"Workspace {workspace.Type} not found");

                if (workspaceType.Count > 1)
                    throw new InvalidOperationException($"Multiple workspaces found with name {workspace.Type}");

                var workspaceInstance = workspaceType[0];
                workspaceInstance.Initialize(workspace.Parameters);
                workspaces.Add(workspaceInstance);
            }

            return workspaces.ToArray();
        }

        private Toolbox CreateTools(ToolDefinition[] toolDefinitions)
        {
            Toolbox toolbox;
            var agentTools = new List<ITool>();

            foreach (var tool in toolDefinitions)
            {
                var agentTool = CreateInstances<ITool>(tool.Name, "Tool");

                if (agentTool.Count == 0)
                    throw new InvalidOperationException($"Tool {tool.Name} not found");

                if (agentTool.Count > 1)
                    throw new InvalidOperationException($"Multiple tools found with name {tool.Name}");

                agentTools.Add(agentTool[0]);
            }

            toolbox = new Toolbox(agentTools.ToArray());
            return toolbox;
        }

        private IReadOnlyList<T> CreateInstances<T>(Func<string, bool> nameFilter = null)
            where T : class
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var instanceTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => nameFilter == null || nameFilter(type.Name))
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

        private IReadOnlyList<T> CreateInstances<T>(string nameFilter, string nameFilterSuffix)
            where T : class
        {
            var filter1 = nameFilter.ToLowerInvariant();
            var filter2 = $"{nameFilter}{nameFilterSuffix}".ToLowerInvariant();
            return CreateInstances<T>(name => name.ToLowerInvariant() == filter1 || name.ToLowerInvariant() == filter2);
        }
    }
}
