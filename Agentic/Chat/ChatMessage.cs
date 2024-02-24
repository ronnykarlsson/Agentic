using System.Diagnostics;

namespace Agentic.Chat
{
    [DebuggerDisplay("{Role,nq} : {Content,nq}")]
    public class ChatMessage
    {
        public ChatMessage(Role role, string content)
        {
            Role = role;
            Content = content;
        }

        public Role Role { get; set; }
        public string Content { get; set; }
    }
}
