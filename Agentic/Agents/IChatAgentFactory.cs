using Agentic.Tools.Confirmation;

namespace Agentic.Agents
{
    public interface IChatAgentFactory
    {
        IChatAgent Create();
        IChatAgent Create(IToolConfirmation toolConfirmation);
        ChainedChatAgent CreateChainedChatAgent(params IChatAgent[] chatAgents);
    }
}