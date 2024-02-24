using System;

namespace Agentic.Agents
{
    public class ChatResponseEventArgs : EventArgs
    {
        public string Response { get; set; }
        public bool IsTool { get; set; }
    }
}