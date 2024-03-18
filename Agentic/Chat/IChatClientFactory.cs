using Agentic.Profiles;

namespace Agentic.Chat
{
    public interface IChatClientFactory
    {
        string Name { get; }
        IChatClient Create();
        IChatClient Create(ClientSettings clientSettings);
    }
}
