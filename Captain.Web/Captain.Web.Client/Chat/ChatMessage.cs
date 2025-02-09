namespace Captain.Web.Client.Chat
{
    public class ChatMessage
    {
        public ChatMessage(string source, string message)
        {
            Source = source;
            Content = message;
        }

        public string Source { get; set; }
        public string Content { get; set; }
    }
}
