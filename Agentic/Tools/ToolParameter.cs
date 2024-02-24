namespace Agentic.Tools
{
    public class ToolParameter<T>
    {
        public T Value { get; set; }

        public ToolParameter()
        {
        }

        public ToolParameter(T value)
        {
            Value = value;
        }
    }
}
