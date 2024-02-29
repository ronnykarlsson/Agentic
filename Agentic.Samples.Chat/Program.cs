using Agentic.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agentic.Sample.Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddAgenticServices()
                .AddSingleton<IChatService, ChatService>()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var chatService = serviceProvider.GetService<IChatService>();

            if (chatService == null) throw new InvalidOperationException($"{nameof(IChatService)} not found");
            chatService.StartChat();
        }
    }
}
