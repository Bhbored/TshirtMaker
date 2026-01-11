window.themeToggle = {
    toggle: function () {
        const html = document.documentElement;
        const currentTheme = html.getAttribute('data-custom-theme');
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        html.setAttribute('data-custom-theme', newTheme);
    },

    getCurrentTheme: function () {
        return document.documentElement.getAttribute('data-custom-theme');
    }
};

window.getScrollInfo = function () {
    const scrollPosition = window.innerHeight + window.scrollY;
    const threshold = document.documentElement.scrollHeight - 500;
    return {
        isNearBottom: scrollPosition >= threshold
    };
};

window.scrollToElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn('Element not found:', elementId);
        return;
    }

    const headerOffset = 80;
    const elementPosition = element.getBoundingClientRect().top;
    const offsetPosition = elementPosition + window.scrollY - headerOffset;

    window.scrollTo({
        top: Math.max(0, offsetPosition),
        behavior: 'smooth'
    });
};
