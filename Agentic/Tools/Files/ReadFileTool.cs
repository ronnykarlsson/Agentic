using System.IO;

namespace Agentic.Tools.Files
{
    public class ReadFileTool : ITool
    {
        public string Tool { get; } = "ReadFile";
        public string Description { get; } = "Reads the contents of a file.";
        public bool RequireConfirmation { get; } = true;

        public ToolParameter<string> Path { get; set; }

        public string Invoke()
        {
            if (!File.Exists(Path.Value))
            {
                return $"File not found: {Path.Value}";
            }

            return File.ReadAllText(Path.Value);
        }
    }
}
