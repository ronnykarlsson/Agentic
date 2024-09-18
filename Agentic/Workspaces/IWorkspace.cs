using Agentic.Chat;
using Agentic.Tools;
using System.Collections.Generic;

namespace Agentic.Workspaces
{
    /// <summary>
    /// Workspace provides a prompt for agents.
    /// </summary>
    public interface IWorkspace
    {
        void Initialize(Dictionary<string, string> parameters);
        string GetPrompt(ChatContext chatContext);
        ITool[] GetWorkspaceTools();
    }
}
