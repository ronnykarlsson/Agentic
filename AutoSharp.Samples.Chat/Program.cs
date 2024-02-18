using AutoSharp.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoSharp.Sample.Chat
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
                .AddAutoSharpServices()
                .AddSingleton<IChatService, ChatService>()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var chatService = serviceProvider.GetService<IChatService>();
            chatService.StartChat();
        }
    }
}
