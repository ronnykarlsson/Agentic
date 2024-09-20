using Agentic.Agents;
using Microsoft.Playwright;

namespace Agentic.Tools.Browser
{
    public class BrowserTextInputTool : BrowserTool
    {
        public override string Tool => "BrowserTextInput";
        public override string Description => "Enter text into an input element on the page";
        public ToolParameter<string> Id { get; set; }
        public ToolParameter<string> Text { get; set; }
        public ToolParameter<bool> PressEnterAfterInput { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(Id.Value))
            {
                return "Error: Element Id is not provided.";
            }

            var selector = GetIdSelector(Id.Value);
            var text = Text.Value ?? "";

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
                Page.FillAsync(selector, text).GetAwaiter().GetResult();
            }
            catch (PlaywrightException ex)
            {
                return $"Error: {ex.Message}";
            }

            if (PressEnterAfterInput.Value)
            {
                try
                {
                    Page.PressAsync(selector, "Enter").GetAwaiter().GetResult();
                }
                catch (PlaywrightException ex)
                {
                    return $"Enter failed: {ex.Message}";
                }
            }

            return ReadPage(Page);
        }
    }
}
