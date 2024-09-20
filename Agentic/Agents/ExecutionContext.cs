using Agentic.Chat;
using Agentic.Workspaces;
using System.Linq;

namespace Agentic.Agents
{
    public class ExecutionContext
    {
        public ChatMessageLinkedList Messages { get; set; }
        public IWorkspace[] Workspaces { get; set; }

        public T GetWorkspace<T>() where T : IWorkspace
        {
            return Workspaces.OfType<T>().FirstOrDefault();
        }
    }
}
