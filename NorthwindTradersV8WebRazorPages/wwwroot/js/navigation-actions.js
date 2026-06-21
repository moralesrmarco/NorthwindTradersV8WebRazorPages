function configurarNavegacion(selector = '.navigation-btn') {

    const buttons = document.querySelectorAll(selector);

    buttons.forEach(btn => {

        btn.addEventListener('click', function () {

            buttons.forEach(b => {
                b.style.pointerEvents = 'none';
            });

            const overlay =
                document.getElementById('loadingOverlay');

            if (overlay) {
                overlay.style.display = 'block';
            }
        });
    });
}