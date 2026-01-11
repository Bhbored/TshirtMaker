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
