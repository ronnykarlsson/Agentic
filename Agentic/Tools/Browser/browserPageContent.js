(function () {
    const isVisible = (el) => {
        if (el.nodeType === Node.TEXT_NODE) {
            el = el.parentNode;
        }
        const style = window.getComputedStyle(el);
        if (style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0' || parseFloat(style.opacity) === 0 || style.fontSize === '0px') {
            return false;
        }
        const rect = el.getBoundingClientRect();
        return rect.width > 0 && rect.height > 0;
    };

    const generateUniqueId = (el) => {
        if (el.nodeType === Node.ELEMENT_NODE && !el.id) {
            const uniqueId = Math.random().toString(36).substr(2, 9);
            el.id = uniqueId;
        }
        return el.id || '';
    };

    const isInteractableElement = (el) => {
        const isInteractable = el.tagName.toLowerCase() === 'a'
            || el.tagName.toLowerCase() === 'button'
            || el.tagName.toLowerCase() === 'input'
            || el.tagName.toLowerCase() === 'textarea'
            || el.hasAttribute('onclick');
        return isInteractable;
    }

    const formatText = (node) => {
        let text = '';
        if (node.nodeType === Node.TEXT_NODE && isVisible(node)) {
            text += node.textContent.trim() + ' ';
        } else if (node.nodeType === Node.ELEMENT_NODE) {
            if (isInteractableElement(node) && isVisible(node)) {
                const id = generateUniqueId(node);

                let parts = [node.tagName.toLowerCase(), `#${id}`];

                if (node.hasAttribute('name')) {
                    parts.push(node.name);
                }

                if (node.hasAttribute('title')) {
                    parts.push(node.title);
                }

                if (node.hasAttribute('href')) {
                    parts.push(node.href);
                }

                if (node.hasAttribute('href') && !node.getAttribute('href').startsWith('mailto:')) {
                    text += `[${node.tagName}:${node.textContent.trim()}](#${id},${node.href})`;
                }

                var textContent = node.textContent.trim();
                if (textContent.length > 0) {
                    parts.push(textContent);
                };

                text += `[${parts.join(',')}]`;
            } else {
                Array.from(node.childNodes).forEach(child => {
                    text += formatText(child);
                });
            }
        }
        return text;
    };

    // Main function to process and format text for the page
    const processPageContent = () => {
        const body = document.body;
        let formattedText = '';

        if (!body) return "No body found";

        Array.from(body.childNodes).forEach(node => {
            formattedText += formatText(node);
        });

        return formattedText.trim();
    };

    return JSON.stringify(processPageContent());
})();
