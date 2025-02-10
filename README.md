# Agentic

Agentic is a .NET framework for programming AI agents with access to tools.

Go to https://www.opencaptain.com to try the tool.

## Example

```c#
var toolbox = new Toolbox(new PwshTool());
var chatAgent = _chatAgentFactory.Create();
chatAgent.Initialize("You are an AI agent, use your tools to answer questions and complete tasks asked of you.", toolbox);

chatAgent.ChatResponse += (sender, eventArgs) => Console.WriteLine(eventArgs.Response);

while (true)
{
    var input = Console.ReadLine();
    chatAgent.ChatAsync(input).GetAwaiter().GetResult();
}
```
