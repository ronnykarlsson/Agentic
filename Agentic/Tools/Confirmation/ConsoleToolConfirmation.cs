using Agentic.Utilities;
using System;

namespace Agentic.Tools.Confirmation
{
    public class ConsoleToolConfirmation : IToolConfirmation
    {
        public bool Confirm(ITool tool)
        {
            Console.WriteLine($"Confirm {tool.Tool}: {ChatHelpers.GetToolJson(tool)}");
            Console.WriteLine($"Continue with tool?");

            Console.Write("[Y] Yes  [N] No  (default is \"Y\"): ");
            var key = Console.ReadKey();
            Console.WriteLine();
            return key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter;
        }
    }
}
