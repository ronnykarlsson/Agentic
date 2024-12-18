using Agentic.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Agentic.CLI
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var options = CommandLineParser.ParseArguments(args);
            Run(options);

            return 0;
        }

        static void Run(Options options)
        {
            // Build configuration
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>();

            // Add API key if provided
            if (!string.IsNullOrEmpty(options.ApiKey))
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { $"Agentic:Client:{nameof(options.ApiKey)}", options.ApiKey }
                });
            }

            var configuration = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection()
                .AddAgenticServices()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(logging =>
                {
                    logging.AddConsole().SetMinimumLevel(options.Verbose ? LogLevel.Debug : LogLevel.Information);
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var agenticFactory = serviceProvider.GetRequiredService<IAgenticFactory>();

            var agent = agenticFactory.CreateFromFile(options.Agent);

            agent.ChatResponse += (sender, eventArgs) =>
            {
                if (eventArgs.IsTool)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                if (!string.IsNullOrEmpty(eventArgs.Agent?.Name))
                {
                    Console.WriteLine($"{eventArgs.Agent.Name} > {eventArgs.Response}");
                }
                else
                {
                    Console.WriteLine(eventArgs.Response);
                }

                Console.ResetColor();
            };

            string? initialPrompt = null;

            // Check if prompt is provided via command-line options
            if (!string.IsNullOrEmpty(options.PromptFile))
            {
                if (File.Exists(options.PromptFile))
                {
                    initialPrompt = File.ReadAllText(options.PromptFile);
                }
                else
                {
                    Console.WriteLine($"Error: Prompt file '{options.PromptFile}' does not exist.");
                    Environment.Exit(1);
                }
            }
            else if (!string.IsNullOrEmpty(options.Prompt))
            {
                initialPrompt = options.Prompt;
            }
            else if (Console.IsInputRedirected)
            {
                // Read from stdin
                using (var reader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding))
                {
                    initialPrompt = reader.ReadToEnd();
                }
            }

            // Send the initial prompt to the agent if provided
            if (!string.IsNullOrEmpty(initialPrompt))
            {
                agent.ChatAsync(initialPrompt).GetAwaiter().GetResult();

                if (options.AbortAfterInitialPrompt)
                {
                    return;
                }
            }

            // Main loop
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (input == null)
                {
                    // End of input (e.g., Ctrl+D), exit the loop
                    break;
                }

                agent.ChatAsync(input).GetAwaiter().GetResult();
            }
        }
    }
}
