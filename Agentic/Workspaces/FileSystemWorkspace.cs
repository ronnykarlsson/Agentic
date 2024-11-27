using Agentic.Agents;
using Agentic.Chat;
using Agentic.Tools;
using Agentic.Tools.Files;
using Agentic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Agentic.Workspaces
{
    /// <summary>
    /// Path for AI to work inside, always shows the contents of the folder.
    /// </summary>
    public class FileSystemWorkspace : IFileSystemWorkspace
    {
        public string BasePath { get; set; }

        public string GetPrompt(ExecutionContext context)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Working directory: {BasePath}");

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }

            stringBuilder.AppendLine("Files:");

            var items = AddFileList(BasePath, stringBuilder);
            if (items == 0) stringBuilder.AppendLine("No files.");

            var fileSystemPrompt = stringBuilder.ToString();
            return fileSystemPrompt;
        }

        public ITool[] GetWorkspaceTools()
        {
            return new ITool[]
            {
                new ReadFileTool(),
                new WriteFileTool(),
                new ListFilesTool()
            };
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            parameters.TryGetValue("path", out string path);
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path is required for FileSystemWorkspace.");

            path = FilePathResolver.ResolvePath(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            BasePath = path;
        }

        public string GetPath(string path)
        {
            var combinedPath = Path.Combine(BasePath, path);

            if (!combinedPath.StartsWith(BasePath, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Path is outside of the workspace.");
            }

            return combinedPath;
        }

        private int AddFileList(string path, StringBuilder stringBuilder, int level = 0)
        {
            var files = Directory.GetFiles(path);
            var itemCount = files.Length;
            foreach (var file in files)
            {
                stringBuilder.AppendLine($"{new string(' ', level * 2)}> {file}");
            }

            var directories = Directory.GetDirectories(path);
            itemCount += directories.Length;
            foreach (var directory in directories)
            {
                stringBuilder.AppendLine($"{new string(' ', level * 2)}/ {directory}");
                itemCount += AddFileList(Path.Combine(path, directory), stringBuilder, level + 1);
            }

            return itemCount;
        }
    }
}
