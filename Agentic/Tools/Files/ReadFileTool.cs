using Agentic.Agents;
using Agentic.Workspaces;
using System.IO;

namespace Agentic.Tools.Files
{
    public class ReadFileTool : ITool
    {
        public string Tool { get; } = "ReadFile";
        public string Description { get; } = "Reads the contents of a file.";
        public bool RequireConfirmation { get; } = true;

        public ToolParameter<string> Path { get; set; }

        public string Invoke(AgentExecutionContext context)
        {
            var path = context.GetWorkspace<IFileSystemWorkspace>()?.GetPath(Path.Value) ?? Path.Value;

            if (!File.Exists(path))
            {
                return $"File not found: {Path.Value}";
            }

            return File.ReadAllText(path);
        }
    }
}
