namespace Agentic.Tools
{
    public interface ITool
    {
        string Name { get; }
        string ParameterName { get; }
        string Description { get; }
        string Invoke(string parameter);
    }
}
