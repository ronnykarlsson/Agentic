using Agentic.Tools;
using Agentic.Workspaces;
using System.Threading.Tasks;

namespace Agentic.Chat
{
    public interface IChatClient
    {
        void SetSystemMessage(string systemMessage);
        void SetTools(Toolbox toolbox);
        void SetWorkspaces(IWorkspace[] workspaces);
        Task<ChatMessage> SendAsync(ChatContext chatContext);
        Task<ChatMessage> SendAsync(ChatContext chatContext, ChatMessage message);
        string LimitMessageSize(string message, float percentMaxTokens);
    }
}
