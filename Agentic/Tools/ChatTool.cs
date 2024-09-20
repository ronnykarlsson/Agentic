using Agentic.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Agentic.Tools
{
    internal class ChatTool : ITool
    {
        public IList<IChatAgent> Agents { get; set; }

        public ChatTool(IList<IChatAgent> agents)
        {
            Description = $"Send message to partner agent: {string.Join(", ", agents.Select(agent => agent.Name))}";
            Agents = agents;
        }

        public string Tool => "Chat";
        public string Description { get; }
        public bool RequireConfirmation => false;

        public ToolParameter<string> Target { get; set; }
        public ToolParameter<string> Message { get; set; }

        public string Invoke(ExecutionContext context)
        {
            if (string.IsNullOrEmpty(Target.Value)) return $"{nameof(Target)} is required";
            if (string.IsNullOrEmpty(Message.Value)) return $"{nameof(Message)} is required";

            var agent = Agents.FirstOrDefault(a => a.Name.Equals(Target.Value, StringComparison.OrdinalIgnoreCase));
            if (agent == null) return $"'{Target.Value}' not found, it can be one of: {string.Join(", ", Agents.Select(a => a.Name))}";

            var response = agent.ChatAsync(Message.Value).GetAwaiter().GetResult();
            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var responseJson = JsonSerializer.Serialize(response, options);

            return $"{{\"agent\":\"{agent.Name}\",\"response\":{responseJson}}}";
        }
    }
}
