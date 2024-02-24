namespace Agentic.Tools.Confirmation
{
    public class AlwaysConfirmToolConfirmation : IToolConfirmation
    {
        public bool Confirm(ITool tool)
        {
            return true;
        }
    }
}