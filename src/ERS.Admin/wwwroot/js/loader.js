(function () {
    var el = document.getElementById('ers-loader');

    var INITIAL_LOAD_DELAY = 500;       
    var LANGUAGE_SWITCH_DELAY = 150; 

    function show() {
        if (!el) return;
        el.classList.remove('is-hidden');
    }

    function hide() {
        if (!el) return;
        el.classList.add('is-hidden');
    }

    window.ersLoader = { show: show, hide: hide };

    window.addEventListener('load', function () {
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                setTimeout(hide, INITIAL_LOAD_DELAY);
            });
        });
    });

    setTimeout(hide, 4000);
})();
