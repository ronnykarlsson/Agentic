using Agentic.Agents;
using Agentic.Chat;
using Agentic.Clients.OpenAI;
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
                .AddScoped<IOpenAIClient, OpenAIClient>()
                .AddTransient<IChatClient, OpenAIChatClient>()
                .AddTransient<IOpenAIChatClient, OpenAIChatClient>()
                .AddScoped<IOpenAIChatClientFactory, OpenAIChatClientFactory>()
                .AddTransient<IChatAgent, ChatAgent>()
                .AddScoped<IChatAgentFactory, ChatAgentFactory>();

            return services;
        }
    }
}
