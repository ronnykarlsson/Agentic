namespace Agentic.Tools.Confirmation
{
    public interface IToolConfirmation
    {
        bool Confirm(ToolInvocation toolInvocation);
    }
}