using System;

namespace Agentic.Tools
{
    public class InlineTool : ITool
    {
        public InlineTool(string name, string parameterName, string description, Func<string, string> action)
        {
            Name = name;
            ParameterName = parameterName;
            Description = description;
            _action = action;
        }

        public string Name { get; }
        public string ParameterName { get; }
        public string Description { get; }

        private Func<string, string> _action;

        public string Invoke(string parameter)
        {
            return _action(parameter);
        }
    }
}
