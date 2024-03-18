using Agentic.Agents;
using Agentic.Chat;
using Agentic.Chat.Ollama;
using Agentic.Chat.OpenAI;
using Agentic.Clients.Ollama;
using Agentic.Clients.OpenAI;
using Agentic.Embeddings;
using Agentic.Embeddings.OpenAI;
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
                .AddTransient<IOpenAIChatClient, OpenAIChatClient>()
                .AddTransient<IOpenAIChatClientFactory, OpenAIChatClientFactory>()
                .AddTransient<IOpenAIEmbeddingsClient, OpenAIEmbeddingsClient>()

                // Ollama
                .AddTransient<IOllamaClient, OllamaClient>()
                .AddTransient<IOllamaChatClient, OllamaChatClient>()
                .AddTransient<IOllamaChatClientFactory, OllamaChatClientFactory>()

                // Default Open AI
                .AddTransient<IChatClientFactory, OpenAIChatClientFactory>()
                .AddTransient<IEmbeddingsClient, OpenAIEmbeddingsClient>()

                .AddTransient<IChatAgent, ChatAgent>()
                .AddTransient<IChatAgentFactory, ChatAgentFactory>()

                .AddTransient<IProfileLoader, ProfileLoader>()
                .AddTransient<IAgenticFactory, AgenticFactory>();

            return services;
        }
    }
}
