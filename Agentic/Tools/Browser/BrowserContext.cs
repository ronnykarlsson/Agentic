using Microsoft.Playwright;

namespace Agentic.Tools.Browser
{
    /// <summary>
    /// Browser tool can only use one browser and one page, keep track of the active one here.
    /// </summary>
    internal class BrowserContext
    {
        public static IBrowser Browser { get; set; }
        public static IPage Page { get; set; }
    }
}
