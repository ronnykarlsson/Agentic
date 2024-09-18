using Agentic.Agents;
using Agentic.Chat;
using Agentic.Clients.Ollama;
using Agentic.Clients.Ollama.API;
using Agentic.Clients.OpenAI;
using Agentic.Clients.OpenAI.API;
using Agentic.Embeddings;
using Agentic.Profiles;
using Agentic.Tools.Confirmation;
using Microsoft.Extensions.DependencyInjection;

namespace Agentic.DependencyInjection
{
    public static class AgenticServiceCollectionExtensions
    {
        public static IServiceCollection AddAgenticServices(this IServiceCollection services)
        {
            services
                .AddTransient<IToolConfirmation, ConsoleToolConfirmation>()

                // Open AI
                .AddTransient<IOpenAIClient, OpenAIClient>()
                .AddTransient<IChatClient, OpenAIChatClient>()
                .AddTransient<IEmbeddingClient, OpenAIEmbeddingsClient>()
                .AddTransient<IChatClientFactory, OpenAIClientFactory>()
                .AddTransient<IEmbeddingClientFactory, OpenAIClientFactory>()

                // Ollama
                .AddTransient<IOllamaClient, OllamaClient>()
                .AddTransient<IChatClient, OllamaChatClient>()
                .AddTransient<IEmbeddingClient, OllamaEmbeddingsClient>()
                .AddTransient<IChatClientFactory, OllamaClientFactory>()
                .AddTransient<IEmbeddingClientFactory, OllamaClientFactory>()

                .AddTransient<IChatAgent, ChatAgent>()
                .AddTransient<IChatAgentFactory, ChatAgentFactory>()

                .AddTransient<IProfileLoader, ProfileLoader>()
                .AddTransient<IAgenticFactory, AgenticFactory>();

            return services;
        }
    }
}
