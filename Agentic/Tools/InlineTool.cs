using Agentic.Agents;
using System;

namespace Agentic.Tools
{
    public class InlineTool : ITool
    {
        private Func<string> _action;

        public InlineTool(string name, string description, Func<string> action)
        {
            Tool = name;
            Description = description;
            _action = action;
        }

        protected InlineTool(string name, string description)
        {
            Tool = name;
            Description = description;
        }

        public string Tool { get; }
        public string Description { get; }
        public bool RequireConfirmation { get; } = true;

        public virtual string Invoke(ExecutionContext context)
        {
            return _action();
        }
    }

    public class InlineTool<T> : InlineTool
    {
        private readonly Func<T, string> _action;

        public InlineTool(string name, string description, Func<T, string> action)
            : base(name, description)
        {
            _action = action;
        }

        public ToolParameter<T> Parameter { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            return _action(Parameter.Value);
        }
    }

    public class InlineTool<T1, T2> : InlineTool
    {
        private readonly Func<T1, T2, string> _action;

        public InlineTool(string name, string description, Func<T1, T2, string> action)
            : base(name, description)
        {
            _action = action;
        }

        public ToolParameter<T1> Parameter1 { get; set; }

        public ToolParameter<T2> Parameter2 { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            return _action(Parameter1.Value, Parameter2.Value);
        }
    }

    public class InlineTool<T1, T2, T3> : InlineTool
    {
        private readonly Func<T1, T2, T3, string> _action;

        public InlineTool(string name, string description, Func<T1, T2, T3, string> action)
            : base(name, description)
        {
            _action = action;
        }

        public ToolParameter<T1> Parameter1 { get; set; }

        public ToolParameter<T2> Parameter2 { get; set; }

        public ToolParameter<T3> Parameter3 { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            return _action(Parameter1.Value, Parameter2.Value, Parameter3.Value);
        }
    }

    public class InlineTool<T1, T2, T3, T4> : InlineTool
    {
        private readonly Func<T1, T2, T3, T4, string> _action;

        public InlineTool(string name, string description, Func<T1, T2, T3, T4, string> action)
            : base(name, description)
        {
            _action = action;
        }

        public ToolParameter<T1> Parameter1 { get; set; }

        public ToolParameter<T2> Parameter2 { get; set; }

        public ToolParameter<T3> Parameter3 { get; set; }

        public ToolParameter<T4> Parameter4 { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            return _action(Parameter1.Value, Parameter2.Value, Parameter3.Value, Parameter4.Value);
        }
    }
}
