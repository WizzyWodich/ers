window.ersLoader = {
    show: function () {
        var el = document.getElementById('ers-loader');
        if (el) el.classList.remove('is-hidden');
    },
    hide: function () {
        var el = document.getElementById('ers-loader');
        if (!el) return;
        el.classList.add('is-hidden');
        setTimeout(function () {
            if (el.parentNode) el.parentNode.removeChild(el);
        }, 500);
    }
};

setTimeout(window.ersLoader.hide, 5000);