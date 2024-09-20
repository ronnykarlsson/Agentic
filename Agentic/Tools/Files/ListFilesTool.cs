using Agentic.Agents;
using Agentic.Workspaces;
using System;
using System.IO;

namespace Agentic.Tools.Files
{
    public class ListFilesTool : ITool
    {
        public string Tool { get; } = "ListFiles";
        public string Description { get; } = "Lists the files in a directory.";
        public bool RequireConfirmation { get; } = true;

        public ToolParameter<string> Path { get; set; }
        public bool Recursive { get; set; }

        public string Invoke(ExecutionContext context)
        {
            if (!Directory.Exists(Path.Value))
            {
                return $"Directory not found: {Path.Value}";
            }

            var path = context.GetWorkspace<FileSystemWorkspace>()?.GetPath(Path.Value) ?? Path.Value;

            var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(path, "*", searchOption);
            return string.Join(Environment.NewLine, files);
        }
    }
}
