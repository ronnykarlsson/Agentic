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

            var chatClientFactories = CreateInstances<IChatClientFactory>();

            var clientSettings = profile.Agent.Client ?? profile.Client;
            var chatClientFactory = chatClientFactories.FirstOrDefault(m => m.Name == clientSettings.Name);

            var chatClient = chatClientFactory.Create(clientSettings);

            var agentSettings = profile.Agent;

            var chatAgent = new ChatAgent(chatClient);
            chatAgent.Initialize(agentSettings.Prompt, Toolbox.Empty);

            return chatAgent;
        }

        private IReadOnlyList<T> CreateInstances<T>() where T : class
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var chatAgentFactoryTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            var chatAgentFactories = new List<T>();
            foreach (var type in chatAgentFactoryTypes)
            {
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, type) as T;
                if (instance != null)
                {
                    chatAgentFactories.Add(instance);
                }
            }

            return chatAgentFactories;
        }
    }
}
