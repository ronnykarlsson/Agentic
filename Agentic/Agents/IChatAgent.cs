using Agentic.Chat;
using Agentic.Tools;
using System;
using System.Threading.Tasks;

namespace Agentic.Agents
{
    public interface IChatAgent
    {
        event EventHandler<ChatResponseEventArgs> ChatResponse;
        Task<string> ChatAsync(string message);
        void Initialize(string systemMessage, params ITool[] tools);
        void SetSystemMessage(string systemMessage);
        void SetTools(ITool[] tools);
        ChatContext GetContext();
        void SetContext(ChatContext context);
    }
}