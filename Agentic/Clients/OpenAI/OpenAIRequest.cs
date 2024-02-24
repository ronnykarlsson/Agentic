using System.Collections.Generic;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIRequest
    {
        public OpenAIRequest(string model, int maxTokens)
        {
            Model = model;
            MaxTokens = maxTokens;
        }

        public string Model { get; set; }
        public List<OpenAIMessage> Messages { get; set; } = new List<OpenAIMessage>();
        public int MaxTokens { get; set; }

        public OpenAIMessage AddMessage(string role, string content)
        {
            var message = new OpenAIMessage(role, content);
            Messages.Add(message);
            return message;
        }
    }
}
