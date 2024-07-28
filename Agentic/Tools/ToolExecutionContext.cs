using Agentic.Workspaces;
using System.Linq;

namespace Agentic.Tools
{
    public class ToolExecutionContext
    {
        public IWorkspace[] Workspaces { get; set; }

        public T GetWorkspace<T>() where T : IWorkspace
        {
            return Workspaces.OfType<T>().FirstOrDefault();
        }
    }
}
