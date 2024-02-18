using AutoSharp.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoSharp.Utilities
{
    public static class ChatHelpers
    {
        public static string ChatEndString { get; } = "---END---";

        public static string CreateDefaultSystemMessage(string systemMessage, ITool[] tools)
        {
            if (tools == null || !tools.Any()) return systemMessage;

            var endingChatMessage = $"When you have finished ALL your current tasks successfully as asked for, or require my input, you MUST say '{ChatEndString}' to let me know that you are done.";

            var toolMessage = GetToolMessage(tools);
            string toolString = tools.Length == 1 ? "tool" : "tools";

            var toolSystemMessage = $"If you need to use a tool, don't say anything else, only use the tool. You MUST use your {toolString} to overcome any of your limitations, find information needed and perform actions required to complete your task. You have access to the following {toolString} which you invoke using JSON:\n{toolMessage}";

            var finalSystemMessage = string.Join("\n", systemMessage, endingChatMessage, toolSystemMessage);

            return finalSystemMessage;
        }

        public static string GetToolMessage(ICollection<ITool> tools)
        {
            if (tools == null || !tools.Any()) return string.Empty;

            var toolsMessage = string.Join("\n", tools.Select(tool => $"{{\"Name\":\"{tool.Name}\",\"Parameter\":\"<{tool.ParameterName}>\"}} : {tool.Description}"));
            return toolsMessage;
        }

        public static string GetToolInvocationMessage(ICollection<ToolInvocation> tools)
        {
            if (tools == null || !tools.Any()) return string.Empty;

            var toolsMessage = string.Join("\n", tools.Select(tool => $"{{\"Name\":\"{tool.Name}\",\"Parameter\":\"{tool.Parameter}\"}}"));
            toolsMessage = toolsMessage.Trim();
            return toolsMessage;
        }

        public static IList<ToolInvocation> ParseTools(string message)
        {
            var tools = new List<ToolInvocation>();
            var matches = Regex.Matches(message, @"\{(\s+)?""Name""(\s+)?:(\s+)?""(?<name>[A-Za-z0-9]+)""(\s+)?,(\s+)?""Parameter"":(\s+)?""(?<parameter>.+?)""(\s+)?\}");
            foreach (Match match in matches)
            {
                tools.Add(new ToolInvocation
                {
                    Name = match.Groups["name"].Value,
                    Parameter = Regex.Unescape(match.Groups["parameter"].Value)
                });
            }
            return tools;
        }

        public static string RemoveChatEndString(string content)
        {
            var updatedContent = Regex.Replace(content, $"['\"]?{ChatEndString}['\"]?", "");
            return updatedContent;
        }
    }
}
