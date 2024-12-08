using Agentic.Tools;
using Agentic.Workspaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agentic.Chat.Middleware
{
    internal class ChatClientWrapper : IChatClient
    {
        private readonly IChatClient _client;
        private readonly List<IChatMiddleware> _middlewares;

        public ChatClientWrapper(IChatClient chatClient)
        {
            _client = chatClient;
            _middlewares = new List<IChatMiddleware>();
        }

        public void Use(IChatMiddleware middleware)
        {
            _middlewares.Add(middleware);
        }

        public string LimitMessageSize(string message, float percentMaxTokens)
        {
            return _client.LimitMessageSize(message, percentMaxTokens);
        }

        public void SetSystemMessage(string systemMessage)
        {
            _client.SetSystemMessage(systemMessage);
        }

        public void SetTools(Toolbox toolbox)
        {
            _client.SetTools(toolbox);
        }

        public void SetWorkspaces(IWorkspace[] workspaces)
        {
            _client.SetWorkspaces(workspaces);
        }

        public async Task<ChatMessage> SendAsync(ChatContext chatContext)
        {
            var middlewareContext = new ChatMiddlewareContext(chatContext);
            await InvokeMiddlewaresAsync(middlewareContext).ConfigureAwait(false);
            return middlewareContext.Response;
        }

        public async Task<ChatMessage> SendAsync(ChatContext chatContext, IList<ChatMessage> messages)
        {
            var middlewareContext = new ChatMiddlewareContext(chatContext, messages);
            await InvokeMiddlewaresAsync(middlewareContext).ConfigureAwait(false);
            return middlewareContext.Response;
        }

        private async Task InvokeMiddlewaresAsync(ChatMiddlewareContext context)
        {
            ChatMiddlewareDelegate handler = async (ctx) =>
            {
                // Send the message to the client as a final action
                if (ctx.AdditionalMessages != null && ctx.AdditionalMessages.Count > 0)
                {
                    ctx.Response = await _client.SendAsync(ctx.ChatContext, ctx.AdditionalMessages).ConfigureAwait(false);
                }
                else
                {
                    ctx.Response = await _client.SendAsync(ctx.ChatContext).ConfigureAwait(false);
                }
            };

            foreach (var middleware in _middlewares.AsEnumerable().Reverse())
            {
                var next = handler;
                handler = (ctx) => middleware.InvokeAsync(ctx, next);
            }

            await handler(context).ConfigureAwait(false);
        }
    }

    internal delegate Task ChatMiddlewareDelegate(ChatMiddlewareContext context);

    internal interface IChatMiddleware
    {
        Task InvokeAsync(ChatMiddlewareContext context, ChatMiddlewareDelegate next);
    }
}
