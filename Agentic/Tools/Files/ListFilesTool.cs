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

        public string Invoke()
        {
            if (!Directory.Exists(Path.Value))
            {
                return $"Directory not found: {Path.Value}";
            }

            var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(Path.Value, "*", searchOption);
            return string.Join(Environment.NewLine, files);
        }
    }
}
