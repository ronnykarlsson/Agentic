using AutoSharp.Agents;
using AutoSharp.Chat;
using AutoSharp.Clients.OpenAI;
using AutoSharp.Tools.Confirmation;
using Microsoft.Extensions.DependencyInjection;

namespace AutoSharp.DependencyInjection
{
    public static class AutoSharpServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoSharpServices(this IServiceCollection services)
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
