using Agentic.Agents;
using Agentic.Clients.Ollama;
using Agentic.Profiles;
using Agentic.Tools;
using Agentic.Tools.Browser;

namespace Agentic.Sample.Chat
{
    public class ChatService : IChatService
    {
        private readonly OllamaClientFactory _chatAgentFactory;

        public ChatService(OllamaClientFactory chatAgentFactory)
        {
            _chatAgentFactory = chatAgentFactory;
        }

        public void StartChat()
        {
            var toolbox = new Toolbox(
                new PwshTool(),
                new BrowserGotoTool(),
                new BrowserClickTool(),
                new BrowserTextInputTool());

            var chatClient = _chatAgentFactory.CreateChatClient(new ClientSettings
            {
                Name = "Ollama",
                Model = "mistral-nemo",
                Tokens = 100000
            });
            var chatAgent = new ChatAgent(chatClient);

            chatAgent.Initialize(
                "As an intelligent assistant, your task is to provide accurate and efficient responses to queries and tasks. When faced with a request, assess whether it falls within your direct knowledge or requires external tools, which I can help access. Specify any needed tools and how to use them, be vigilant about correcting mistakes with my assistance, and learn from each interaction to improve future responses. Communicate your process and instructions clearly, aiming for the best outcome. Your goal is to effectively combine your capabilities with external tools through me, ensuring precision and efficiency. For external resources, only use APIs which doesn't require a key.",
                toolbox, null);

            chatAgent.ChatResponse += (sender, eventArgs) =>
            {
                if (eventArgs.IsTool)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.WriteLine(eventArgs.Response);

                Console.ResetColor();
            };

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                var chatResponse = chatAgent.ChatAsync(input).GetAwaiter().GetResult();
            }
        }
    }
}
