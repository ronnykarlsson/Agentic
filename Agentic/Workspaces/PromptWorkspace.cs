using Agentic.Agents;
using Agentic.Chat;
using Agentic.Tools;
using System.Collections.Generic;

namespace Agentic.Workspaces
{
    /// <summary>
    /// Simple workspace which provides a common prompt to agents
    /// </summary>
    public class PromptWorkspace : IWorkspace
    {
        public string Prompt { get; set; }

        public void Initialize(Dictionary<string, string> parameters)
        {
            parameters.TryGetValue("prompt", out string prompt);
            Prompt = prompt;
        }

        public string GetPrompt(AgentExecutionContext context)
        {
            return Prompt;
        }

        public ITool[] GetWorkspaceTools()
        {
            return null;
        }
    }
}
