using Microsoft.Playwright;

namespace Agentic.Tools.Browser
{
    public abstract class BrowserTool : ITool
    {
        public abstract string Tool { get; }
        public abstract string Description { get; }
        public virtual bool RequireConfirmation => false;

        public abstract string Invoke();

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

        protected string ReadPage(IPage page)
        {
            var title = page.TitleAsync().GetAwaiter().GetResult();

            var contentScript = @"
(function() {
    const isVisible = (el) => {
        const style = window.getComputedStyle(el);
        if (style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0' || style.fontSize === '0px') return false;
        const rect = el.getBoundingClientRect();
        if (rect.width === 0 || rect.height === 0) return false;
        return document.elementFromPoint(rect.left + rect.width / 2, rect.top + rect.height / 2) === el;
    };

    const generateUniqueId = (el) => {
        if (el.id) return el.id;
        const uniqueId = '_id_' + Math.random().toString(36).substr(2, 9);
        el.id = uniqueId;
        return uniqueId;
    };

    const formatElementText = (el) => {
        let text = el.innerText; // Removed the trim here for safety
        if (text === undefined) return ''; // Check for undefined explicitly
        text = text.trim(); // Perform the trim after the check
        const id = generateUniqueId(el);
        if (el.tagName.toLowerCase() === 'a' || el.tagName.toLowerCase() === 'button' || el.tagName.toLowerCase() === 'input') {
            return `[${text}](#${id})`;
        }
        return text;
    };

    const uniqueTexts = new Map();

    document.querySelectorAll('body *:not(script):not(style)').forEach(el => {
        if (!isVisible(el)) return;

        const text = formatElementText(el);
        if (text.length <= 1) return;

        const path = [];
        let parent = el;
        while (parent !== document.body) {
            path.unshift(`${parent.tagName.toLowerCase()}${parent.className ? '.' + parent.className.split(' ').join('.') : ''}`);
            parent = parent.parentNode;
        }

        const selector = path.join(' > ');
        if (!uniqueTexts.has(selector)) {
            uniqueTexts.set(selector, text);
        }
    });

    return JSON.stringify(Array.from(uniqueTexts.values()).join('\\n\\n'));
})();
";

            var textContent = page.EvaluateAsync<string>(contentScript).GetAwaiter().GetResult();

            var elementsScript = @"
(function() {
    const isVisible = (el) => {
        const style = window.getComputedStyle(el);
        if (style.display === 'none' || style.visibility === 'hidden') return false;
        const rect = el.getBoundingClientRect();
        if (rect.width === 0 || rect.height === 0) return false;
        return true;
    };

    const generateRandomId = () => '_id_' + Math.random().toString(36).substr(2, 9);

    const elements = document.querySelectorAll('a, button, input:not([type=hidden]), textarea, select, [tabindex]:not([tabindex=""-1""])');
    return JSON.stringify([...elements].filter(isVisible).map((el, index) => {
        const textContent = el.innerText || el.value || el.title || '';
        const text = textContent ? textContent.trim() : '';
        let id = el.id;
        if (!id) {
            id = generateRandomId();
            el.id = id; // Set the random ID on the element
        }
        const type = el.tagName.toLowerCase();
        const href = el.tagName.toLowerCase() === 'a' ? el.getAttribute('href') : '';
        if (href) return { type, text, id, href };
        return { type, text, id };
    }).filter(el => el.text && el.text.length > 0));
})();
";

            var elements = page.EvaluateAsync<string>(elementsScript).GetAwaiter().GetResult();

            var resultJson = $"{{\"title\": \"{title.Replace("\"", "\\\"")}\", \"textContent\": {textContent}, \"interactableElements\": {elements}}}";

            return resultJson;
        }
    }
}
