using System;

namespace AutoSharp.Tools.Confirmation
{
    public class ConsoleToolConfirmation : IToolConfirmation
    {
        public bool Confirm(ToolInvocation toolInvocation)
        {
            Console.WriteLine($"{toolInvocation.Name}: {toolInvocation.Parameter}");
            Console.WriteLine($"Continue with tool?");
            Console.Write("[Y] Yes  [N] No  (default is \"Y\"): ");
            var key = Console.ReadKey();
            Console.WriteLine();
            return key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter;
        }
    }
}
