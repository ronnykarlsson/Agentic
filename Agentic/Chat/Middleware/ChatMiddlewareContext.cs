using System.Collections.Generic;
using System.Linq;

namespace Agentic.Chat.Middleware
{
    internal class ChatMiddlewareContext
    {
        public ChatContext ChatContext { get; }
        public ChatMessage Response { get; set; }
        public List<ChatMessage> AdditionalMessages { get; set; } = new List<ChatMessage>();

        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();

        public ChatMiddlewareContext()
        {
            ChatContext = new ChatContext();
        }

        public ChatMiddlewareContext(ChatContext chatContext)
        {
            ChatContext = chatContext;
        }

        public ChatMiddlewareContext(ChatContext chatContext, ICollection<ChatMessage> messages)
        {
            ChatContext = chatContext;
            AdditionalMessages.AddRange(messages);
        }

        private ChatMessage GetSentMessage(ChatContext chatContext)
        {
            var message = chatContext.Messages.LastOrDefault(m => m.Role != Role.System);
            if (message == null) message = new ChatMessage(Role.User, string.Empty);
            return message;
        }
    }
}
