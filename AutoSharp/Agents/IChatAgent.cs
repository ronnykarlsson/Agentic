using AutoSharp.Chat;
using AutoSharp.Tools;
using System;
using System.Threading.Tasks;

namespace AutoSharp.Agents
{
    public interface IChatAgent
    {
        event EventHandler<string> ChatResponse;
        Task<string> ChatAsync(string message);
        void Initialize(string systemMessage, params ITool[] tools);
        void SetSystemMessage(string systemMessage);
        void SetTools(ITool[] tools);
        ChatContext GetContext();
        void SetContext(ChatContext context);
    }
}