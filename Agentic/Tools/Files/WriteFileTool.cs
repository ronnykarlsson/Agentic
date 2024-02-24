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

        public string Invoke()
        {
            var directory = System.IO.Path.GetDirectoryName(Path.Value);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(Path.Value, Content.Value);
            return $"File written: {Path.Value}";
        }
    }
}
