using System.Diagnostics;

namespace Agentic.Clients.OpenAI.API
{
    [DebuggerDisplay("{Role,nq} : {Content,nq}")]
    public class OpenAIMessage
    {
        public string Role { get; private set; }
        public string Content { get; private set; }

        public OpenAIMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
