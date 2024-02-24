using Agentic.Agents;
using Agentic.Tools;

namespace Agentic.Sample.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatAgentFactory _chatAgentFactory;

        public ChatService(IChatAgentFactory chatAgentFactory)
        {
            _chatAgentFactory = chatAgentFactory;
        }

        public void StartChat()
        {
            var tools = new ITool[] { new PwshTool() };

            var chatAgent = _chatAgentFactory.Create();

            chatAgent.Initialize(
                "As an intelligent assistant, your task is to provide accurate and efficient responses to queries and tasks. When faced with a request, assess whether it falls within your direct knowledge or requires external tools, which I can help access. Specify any needed tools and how to use them, be vigilant about correcting mistakes with my assistance, and learn from each interaction to improve future responses. Communicate your process and instructions clearly, aiming for the best outcome. Your goal is to effectively combine your capabilities with external tools through me, ensuring precision and efficiency. For external resources, only use APIs which doesn't require a key.",
                tools);

            chatAgent.ChatResponse += (sender, response) => Console.WriteLine(response);

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                var chatResponse = chatAgent.ChatAsync(input).GetAwaiter().GetResult();
            }
        }
    }
}
