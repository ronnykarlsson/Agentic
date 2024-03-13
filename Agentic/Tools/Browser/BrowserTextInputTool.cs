namespace Agentic.Tools.Browser
{
    public class BrowserTextInputTool : BrowserTool
    {
        public override string Tool => "BrowserTextInput";
        public override string Description => "Enter text into an input element on the page";
        public ToolParameter<string> Id { get; set; }
        public ToolParameter<string> Text { get; set; }
        public ToolParameter<bool> PressEnterAfterInput { get; set; }

        public override string Invoke()
        {
            if (string.IsNullOrWhiteSpace(Id.Value))
            {
                return "Error: Element Id is not provided.";
            }

            var selector = Id.Value.StartsWith("#") ? Id.Value : $"#{Id.Value}";
            var text = Text.Value ?? "";

            Page.WaitForSelectorAsync(selector).GetAwaiter().GetResult();
            Page.FillAsync(selector, text).GetAwaiter().GetResult();

            if (PressEnterAfterInput.Value)
            {
                Page.PressAsync(selector, "Enter").GetAwaiter().GetResult();
            }

            return ReadPage(Page);
        }
    }
}
