using Agentic.Chat;
using Agentic.Tools.Confirmation;

namespace Agentic.Agents
{
    public class ChatAgentFactory : IChatAgentFactory
    {
        private readonly IChatClientFactory _clientFactory;
        private readonly IToolConfirmation _toolConfirmation;

        public ChatAgentFactory(IChatClientFactory clientFactory, IToolConfirmation toolConfirmation)
        {
            _clientFactory = clientFactory;
            _toolConfirmation = toolConfirmation;
        }

        public IChatAgent Create()
        {
            return new ChatAgent(_clientFactory.Create(), _toolConfirmation);
        }

        public IChatAgent Create(IToolConfirmation toolConfirmation)
        {
            return new ChatAgent(_clientFactory.Create(), toolConfirmation);
        }

        public ChainedChatAgent CreateChainedChatAgent(params IChatAgent[] chatAgents)
        {
            return new ChainedChatAgent(chatAgents);
        }
    }
}
