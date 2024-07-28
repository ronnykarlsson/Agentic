using Microsoft.Playwright;

namespace Agentic.Tools.Browser
{
    public class BrowserClickTool : BrowserTool
    {
        public override string Tool => "BrowserClick";
        public override string Description => "Click element on page";
        public ToolParameter<string> Id { get; set; }

        public override string Invoke(ToolExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(Id.Value))
            {
                return "Error: Element Id is not provided.";
            }

            var selector = GetIdSelector(Id.Value);

            try
            {
                Page.WaitForSelectorAsync(selector).GetAwaiter().GetResult();
            }
            catch (PlaywrightException ex)
            {
                return $"Error: {ex.Message}";
            }

            try
            {
                Page.ClickAsync(selector).GetAwaiter().GetResult();
            }
            catch (PlaywrightException ex)
            {
                return $"Error: {ex.Message}";
            }

            return ReadPage(Page);
        }
    }
}
