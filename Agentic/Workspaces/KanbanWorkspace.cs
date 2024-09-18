using Agentic.Chat;
using Agentic.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agentic.Workspaces
{
    /// <summary>
    /// Kanban board and tools to manage it
    /// </summary>
    public class KanbanWorkspace : IWorkspace
    {
        public string Name { get; set; }

        public List<KanbanWorkItem> WorkItems { get; } = new List<KanbanWorkItem>();

        public string GetPrompt(ChatContext chatContext)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Name);

            if (!WorkItems.Any())
            {
                stringBuilder.AppendLine("No tickets.");
                return stringBuilder.ToString();
            }

            stringBuilder.AppendLine("Tickets:");
            foreach (var workItem in WorkItems)
            {
                stringBuilder.AppendLine($"- {workItem.Name}");
            }

            return stringBuilder.ToString();
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            parameters.TryGetValue("name", out string name);
            Name = string.IsNullOrWhiteSpace(name) ? "Kanban board" : name;
        }

        public ITool[] GetWorkspaceTools()
        {
            return new ITool[]
            {
                new AddKanbanTicketTool(this),
                new CompleteKanbanTicketTool(this)
            };
        }

        public class AddKanbanTicketTool : ITool
        {
            private KanbanWorkspace _kanbanWorkspace;

            public AddKanbanTicketTool()
            {
            }

            public AddKanbanTicketTool(KanbanWorkspace kanbanWorkspace)
            {
                _kanbanWorkspace = kanbanWorkspace;
            }

            public string Tool => "AddKanbanTicket";
            public string Description => "Add new ticket to the Kanban board";
            public bool RequireConfirmation => false;

            public ToolParameter<string> TicketName { get; set; }
            public ToolParameter<string> TicketDescription { get; set; }

            public string Invoke(ToolExecutionContext context)
            {
                var workspace = _kanbanWorkspace ?? context.GetWorkspace<KanbanWorkspace>();
                if (workspace == null) return "No Kanban workspace found";

                workspace.WorkItems.Add(new KanbanWorkItem
                {
                    Name = TicketName.Value,
                    Description = TicketDescription.Value
                });

                return $"Ticket {TicketName.Value} added";
            }
        }

        public class CompleteKanbanTicketTool : ITool
        {
            private KanbanWorkspace _kanbanWorkspace;

            public CompleteKanbanTicketTool()
            {
            }

            public CompleteKanbanTicketTool(KanbanWorkspace kanbanWorkspace)
            {
                _kanbanWorkspace = kanbanWorkspace;
            }

            public string Tool => "CompleteKanbanTicket";
            public string Description => "Mark a ticket as complete on the Kanban board";
            public bool RequireConfirmation => false;

            public ToolParameter<string> TicketName { get; set; }

            public string Invoke(ToolExecutionContext context)
            {
                var workspace = _kanbanWorkspace ?? context.GetWorkspace<KanbanWorkspace>();
                if (workspace == null) return "No Kanban workspace found";

                var ticket = workspace.WorkItems.Find(w => w.Name == TicketName.Value);
                if (ticket == null) return $"Ticket {TicketName.Value} not found";

                workspace.WorkItems.Remove(ticket);

                return $"Ticket {TicketName.Value} completed";
            }
        }

        public class KanbanWorkItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
