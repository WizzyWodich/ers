window.ersTheme = {
    getStored: function () { return localStorage.getItem('ers-student-theme'); },
    setStored: function (value) { localStorage.setItem('ers-student-theme', value); },
    isSystemDark: function () { return window.matchMedia('(prefers-color-scheme: dark)').matches; },
    apply: function (isDark) { document.documentElement.classList.toggle('dark', isDark); },
    applySmooth: function (isDark) {
        const root = document.documentElement;
        root.classList.add('theme-transition');
        root.classList.toggle('dark', isDark);
        window.clearTimeout(window.__ersThemeTimeout);
        window.__ersThemeTimeout = window.setTimeout(function () { root.classList.remove('theme-transition'); }, 400);
    },
    getBrowserLanguage: function () { return (navigator.language || navigator.userLanguage || 'en').toLowerCase(); }
};