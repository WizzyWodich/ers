window.ersAdminTheme = {
    getStored: function () {
        return localStorage.getItem('ers-admin-theme');
    },

    setStored: function (value) {
        localStorage.setItem('ers-admin-theme', value);
    },

    isSystemDark: function () {
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    },

    apply: function (isDark) {
        document.documentElement.classList.toggle('dark', isDark);
    },

    applySmooth: function (isDark) {
        const root = document.documentElement;
        root.classList.add('theme-transition');
        root.classList.toggle('dark', isDark);
        window.clearTimeout(window.__ersAdminThemeTimeout);
        window.__ersAdminThemeTimeout = window.setTimeout(function () {
            root.classList.remove('theme-transition');
        }, 400);
    }
};