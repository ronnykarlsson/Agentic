using System;
using System.Collections.Generic;
using System.Text;

namespace Agentic.Utilities
{
    public static class ToolHelpers
    {
        public static string StripAnsiColorCodes(string text)
        {
            // ANSI color code pattern
            var ansiCodePattern = @"\x1B\[[0-9;]*m";
            return System.Text.RegularExpressions.Regex.Replace(text, ansiCodePattern, string.Empty);
        }
    }
}
