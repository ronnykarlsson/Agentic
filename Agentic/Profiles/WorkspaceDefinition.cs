using System.Collections.Generic;

namespace Agentic.Profiles
{
    public class WorkspaceDefinition
    {
        public string Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
