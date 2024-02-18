namespace AutoSharp.Chat
{
    public interface IOpenAIChatClientFactory
    {
        IOpenAIChatClient Create();
    }
}