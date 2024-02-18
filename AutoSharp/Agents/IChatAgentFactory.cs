using AutoSharp.Tools.Confirmation;

namespace AutoSharp.Agents
{
    public interface IChatAgentFactory
    {
        IChatAgent Create();
        IChatAgent Create(IToolConfirmation toolConfirmation);
        ChainedChatAgent CreateChainedChatAgent(params IChatAgent[] chatAgents);
    }
}