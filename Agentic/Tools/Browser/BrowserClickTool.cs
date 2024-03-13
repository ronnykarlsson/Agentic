namespace Agentic.Tools.Browser
{
    public class BrowserClickTool : BrowserTool
    {
        public override string Tool => "BrowserClick";
        public override string Description => "Click element on page";
        public ToolParameter<string> Id { get; set; }

        public override string Invoke()
        {
            if (string.IsNullOrWhiteSpace(Id.Value))
            {
                return "Error: Element Id is not provided.";
            }

            var selector = Id.Value.StartsWith("#") ? Id.Value : $"#{Id.Value}";

            Page.WaitForSelectorAsync(selector).GetAwaiter().GetResult();

            Page.ClickAsync(selector).GetAwaiter().GetResult();

            return ReadPage(Page);
        }
    }
}
