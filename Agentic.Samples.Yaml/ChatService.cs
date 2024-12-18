namespace Agentic.Samples.Yaml
{
    public class ChatService : IChatService
    {
        private readonly IAgenticFactory _agenticFactory;

        public ChatService(IAgenticFactory agenticFactory)
        {
            _agenticFactory = agenticFactory;
        }

        public void StartChat()
        {
            var chatAgent = _agenticFactory.CreateFromFile("ragAgent.yml");

            chatAgent.ChatResponse += (sender, eventArgs) =>
            {
                if (eventArgs.IsTool)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                if (!string.IsNullOrEmpty(eventArgs.Agent?.Name))
                {
                    Console.WriteLine($"{eventArgs.Agent.Name} > {eventArgs.Response}");
                }
                else
                {
                    Console.WriteLine(eventArgs.Response);
                }

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
