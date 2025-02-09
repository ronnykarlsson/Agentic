export function scrollToBottom(element) {
    if (element) {
        element.scrollTo({ top: element.scrollHeight, behavior: 'smooth' });
    }
}
