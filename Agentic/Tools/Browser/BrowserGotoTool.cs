using Agentic.Agents;
using Microsoft.Playwright;

namespace Agentic.Tools.Browser
{
    public class BrowserGotoTool : BrowserTool
    {
        public override string Tool => "Browser";
        public override string Description => "Navigate to URL using browser";
        public ToolParameter<string> Url { get; set; }

        public override string Invoke(ExecutionContext context)
        {
            var playwright = Playwright.CreateAsync().GetAwaiter().GetResult();

            if (Browser == null)
            {
                Browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false
                }).GetAwaiter().GetResult();
            }

            if (Page == null)
            {
                Page = Browser.NewPageAsync().GetAwaiter().GetResult();
            }

            Page.GotoAsync(Url.Value).GetAwaiter().GetResult();
            return ReadPage(Page);
        }
    }
}
