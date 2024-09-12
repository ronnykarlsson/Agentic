using Agentic.Profiles;

namespace Agentic.Chat
{
    public interface IChatClientFactory
    {
        string Name { get; }
        IChatClient CreateChatClient();
        IChatClient CreateChatClient(ClientSettings clientSettings);
    }
}
