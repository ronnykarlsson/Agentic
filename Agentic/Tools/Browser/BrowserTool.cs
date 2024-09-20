using Agentic.Agents;
using Microsoft.Playwright;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Agentic.Tools.Browser
{
    public abstract class BrowserTool : ITool
    {
        public abstract string Tool { get; }
        public abstract string Description { get; }
        public virtual bool RequireConfirmation => false;

        public abstract string Invoke(ExecutionContext context);

        protected IBrowser Browser
        {
            get { return BrowserContext.Browser; }
            set { BrowserContext.Browser = value; }
        }

        protected IPage Page
        {
            get { return BrowserContext.Page; }
            set { BrowserContext.Page = value; }
        }

        protected string GetIdSelector(string id)
        {
            var selector = id.StartsWith("#") ? id : $"#{id}";
            return selector;
        }

        protected string ReadPage(IPage page)
        {
            page.WaitForSelectorAsync("body").GetAwaiter().GetResult();

            var title = page.TitleAsync().GetAwaiter().GetResult();

            string contentScript;

            var resourceManifestName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(m => m.EndsWith(".browserPageContent.js"));
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceManifestName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    contentScript = reader.ReadToEnd();
                }
            }

            var textContent = page.EvaluateAsync<string>(contentScript).GetAwaiter().GetResult();

            var resultJson = $"{{\"title\": \"{title.Replace("\"", "\\\"")}\", \"textContent\": {textContent}}}";

            return resultJson;
        }
    }
}
