using Agentic.Chat;
using Agentic.Tools;
using Agentic.Workspaces;
using System;
using System.Threading.Tasks;

namespace Agentic.Agents
{
    public interface IChatAgent
    {
        /// <summary>
        /// Name of this agent.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Callback for each chat response.
        /// </summary>
        event EventHandler<ChatResponseEventArgs> ChatResponse;

        /// <summary>
        /// Toolbox used by the agent.
        /// </summary>
        Toolbox Toolbox { get; }

        /// <summary>
        /// Initialize the agent with system message and tools.
        /// </summary>
        /// <param name="systemMessage">System message to set.</param>
        /// <param name="toolbox">Tools which the agent can use.</param>
        /// <param name="workspaces">Workspaces with common tools and prompts.</param>
        void Initialize(string systemMessage, Toolbox toolbox, IWorkspace[] workspaces);

        /// <summary>
        /// Send <paramref name="message"/> to the chat client and return the response.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>Response from agent</returns>
        Task<string> ChatAsync(string message);

        /// <summary>
        /// Get the chat context.
        /// </summary>
        /// <returns><see cref="ChatContext"/></returns>
        ChatContext GetContext();

        /// <summary>
        /// Set the chat context.
        /// </summary>
        /// <param name="context">Set <paramref name="context"/> for the agent.</param>
        void SetContext(ChatContext context);
    }
}