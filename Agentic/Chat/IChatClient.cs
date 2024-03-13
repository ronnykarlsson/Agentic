using Agentic.Tools;
using System.Threading.Tasks;

namespace Agentic.Chat
{
    public interface IChatClient
    {
        void SetSystemMessage(string systemMessage);
        void SetTools(Toolbox toolbox);
        Task<ChatMessage> SendAsync(ChatContext context);
        Task<ChatMessage> SendAsync(ChatContext context, ChatMessage message);
        string LimitMessageSize(string message, float percentMaxTokens);
    }
}
