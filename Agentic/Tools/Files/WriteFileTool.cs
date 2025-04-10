﻿using Agentic.Agents;
using Agentic.Workspaces;
using System.IO;

namespace Agentic.Tools.Files
{
    public class WriteFileTool : ITool
    {
        public string Tool { get; } = "WriteFile";
        public string Description { get; } = "Writes content to a file.";
        public bool RequireConfirmation { get; } = true;

        public ToolParameter<string> Path { get; set; }
        public ToolParameter<string> Content { get; set; }

        public string Invoke(AgentExecutionContext context)
        {
            var path = context.GetWorkspace<IFileSystemWorkspace>()?.GetPath(Path.Value) ?? Path.Value;

            var directory = System.IO.Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, Content.Value);
            return $"File written: {Path.Value}";
        }
    }
}
