(function () {
    function hideLoader() {
        var el = document.getElementById('ers-loader');
        if (!el || el.classList.contains('is-hidden')) return;
        el.classList.add('is-hidden');
        setTimeout(function () {
            if (el.parentNode) el.parentNode.removeChild(el);
        }, 500);
    }

    window.addEventListener('load', function () {
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                setTimeout(hideLoader, 200);
            });
        });
    });

    setTimeout(hideLoader, 4000);
})();