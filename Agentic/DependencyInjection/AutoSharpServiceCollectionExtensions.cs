using Agentic.Agents;
using Agentic.Chat;
using Agentic.Chat.Ollama;
using Agentic.Chat.OpenAI;
using Agentic.Clients.Ollama;
using Agentic.Clients.OpenAI;
using Agentic.Embeddings;
using Agentic.Embeddings.OpenAI;
using Agentic.Tools.Confirmation;
using Microsoft.Extensions.DependencyInjection;

namespace Agentic.DependencyInjection
{
    public static class AgenticServiceCollectionExtensions
    {
        public static IServiceCollection AddAgenticServices(this IServiceCollection services)
        {
            services
                .AddScoped<IToolConfirmation, ConsoleToolConfirmation>()

                // Open AI
                .AddScoped<IOpenAIClient, OpenAIClient>()
                .AddTransient<IOpenAIChatClient, OpenAIChatClient>()
                .AddScoped<IOpenAIChatClientFactory, OpenAIChatClientFactory>()
                .AddTransient<IOpenAIEmbeddingsClient, OpenAIEmbeddingsClient>()

                // Ollama
                .AddScoped<IOllamaClient, OllamaClient>()
                .AddTransient<IOllamaChatClient, OllamaChatClient>()
                .AddScoped<IOllamaChatClientFactory, OllamaChatClientFactory>()

                // Default Open AI
                .AddScoped<IChatClientFactory, OpenAIChatClientFactory>()
                .AddTransient<IEmbeddingsClient, OpenAIEmbeddingsClient>()

                .AddTransient<IChatAgent, ChatAgent>()
                .AddScoped<IChatAgentFactory, ChatAgentFactory>();

            return services;
        }
    }
}
