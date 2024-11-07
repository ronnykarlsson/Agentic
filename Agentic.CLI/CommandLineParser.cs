using System.Reflection;

namespace Agentic.CLI
{
    static class CommandLineParser
    {
        private static readonly List<OptionDefinition> _optionDefinitions = GetOptionDefinitions();

        public static Options ParseArguments(string[] args)
        {
            var options = new Options();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var option = _optionDefinitions.FirstOrDefault(o => o.Aliases.Contains(arg, StringComparer.OrdinalIgnoreCase));

                if (option != null)
                {
                    if (option.RequiresValue)
                    {
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                        {
                            var value = args[i + 1];
                            option.SetValue(value, options);
                            i++; // Skip the next argument since it's a value
                        }
                        else
                        {
                            Console.WriteLine($"Error: Option '{arg}' requires a value.");
                            ShowHelp();
                            Environment.Exit(1);
                        }
                    }
                    else
                    {
                        option.SetValue(null, options);
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown option: '{arg}'");
                    ShowHelp();
                    Environment.Exit(1);
                }
            }

            foreach (var requiredOption in _optionDefinitions.Where(o => o.IsRequired))
            {
                var property = typeof(Options).GetProperty(requiredOption.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var value = property?.GetValue(options);
                if (value == null || (value is string str && string.IsNullOrEmpty(str)))
                {
                    Console.WriteLine($"Error: The '{requiredOption.Name}' option is required.");
                    ShowHelp();
                    Environment.Exit(1);
                }
            }

            // Handle conflict between --prompt and --prompt-file
            if (!string.IsNullOrEmpty(options.Prompt) && !string.IsNullOrEmpty(options.PromptFile))
            {
                Console.WriteLine("Error: Cannot specify both --prompt and --prompt-file options.");
                ShowHelp();
                Environment.Exit(1);
            }

            return options;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Agen - Command-line interface for AI agents");
            Console.WriteLine();
            Console.Write("Usage:");
            Console.Write("  agen");

            // Build usage line with required options
            foreach (var option in _optionDefinitions.Where(o => o.IsRequired))
            {
                var placeholder = option.RequiresValue ? " <value>" : "";
                Console.Write($" {option.Aliases[0]}{placeholder}");
            }
            Console.WriteLine(" [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");

            // Compute max length for formatting
            int maxAliasLength = _optionDefinitions.Max(o =>
            {
                var aliases = string.Join(", ", o.Aliases);
                var placeholder = o.RequiresValue ? " <value>" : "";
                return aliases.Length + placeholder.Length;
            });

            // Generate options help text
            foreach (var option in _optionDefinitions)
            {
                var aliases = string.Join(", ", option.Aliases);
                var placeholder = option.RequiresValue ? " <value>" : "";
                var aliasText = $"{aliases}{placeholder}";
                Console.WriteLine($"  {aliasText.PadRight(maxAliasLength + 2)}{option.Description}");
            }
        }

        private static List<OptionDefinition> GetOptionDefinitions()
        {
            return new List<OptionDefinition>
            {
                new OptionDefinition
                {
                    Name = "agent",
                    Aliases = ["-a", "--agent"],
                    RequiresValue = true,
                    IsRequired = true,
                    Description = "Path to the agent configuration file (e.g., agent.yml). Required.",
                    SetValue = (val, options) => options.Agent = val
                },
                new OptionDefinition
                {
                    Name = "apikey",
                    Aliases = ["-k", "--apikey"],
                    RequiresValue = true,
                    Description = "Your API key.",
                    SetValue = (val, options) => options.ApiKey = val
                },
                new OptionDefinition
                {
                    Name = "prompt",
                    Aliases = ["-p", "--prompt"],
                    RequiresValue = true,
                    Description = "Initial prompt text to send to the agent.",
                    SetValue = (val, options) => options.Prompt = val
                },
                new OptionDefinition
                {
                    Name = "promptfile",
                    Aliases = ["--prompt-file"],
                    RequiresValue = true,
                    Description = "Path to the prompt file containing the initial prompt text.",
                    SetValue = (val, options) => options.PromptFile = val
                },
                new OptionDefinition
                {
                    Name = "verbose",
                    Aliases = ["-v", "--verbose"],
                    RequiresValue = false,
                    Description = "Enable verbose logging.",
                    SetValue = (val, options) => options.Verbose = true
                },
                new OptionDefinition
                {
                    Name = "help",
                    Aliases = ["-h", "--help"],
                    RequiresValue = false,
                    Description = "Show this help message.",
                    SetValue = (val, options) =>
                    {
                        ShowHelp();
                        Environment.Exit(0);
                    }
                },
                new OptionDefinition
                {
                    Name = "abort",
                    Aliases = ["--abort"],
                    RequiresValue = false,
                    Description = "Abort after processing the initial prompt.",
                    SetValue = (val, options) => options.AbortAfterInitialPrompt = true
                }
            };
        }
    }

    class OptionDefinition
    {
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public bool RequiresValue { get; set; }
        public bool IsRequired { get; set; } = false;
        public string Description { get; set; }
        public Action<string, Options> SetValue { get; set; }
    }

    class Options
    {
        public string Agent { get; set; }
        public string ApiKey { get; set; }
        public bool Verbose { get; set; }
        public string Prompt { get; set; }
        public string PromptFile { get; set; }
        public bool AbortAfterInitialPrompt { get; set; }
    }
}
