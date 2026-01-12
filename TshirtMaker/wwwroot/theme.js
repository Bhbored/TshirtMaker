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

// Scroll-triggered fade-in animations
window.initScrollAnimations = function () {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    // Observe all sections that should fade in
    const sections = document.querySelectorAll('.how-it-works-section, .describe-vision-section, .save-share-section, #features');
    sections.forEach(section => {
        observer.observe(section);
    });
};

// Initialize on page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', window.initScrollAnimations);
} else {
    window.initScrollAnimations();
}
