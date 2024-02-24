namespace Agentic.Tools
{
    public interface ITool
    {
        string Tool { get; }
        string Description { get; }
        string Invoke();
    }
}
