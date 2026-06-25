function mostrarLoadingOverlay() {

    const overlay = document.getElementById("loadingOverlay");

    if (overlay) {
        overlay.style.display = "block";
    }
}

function ocultarLoadingOverlay() {

    const overlay = document.getElementById("loadingOverlay");

    if (overlay) {
        overlay.style.display = "none";
    }
}

document.addEventListener("DOMContentLoaded", function () {

    // Compatibilidad con páginas actuales
    const formulario = document.querySelector("form");

    if (formulario) {
        formulario.addEventListener("submit", function () {
            mostrarLoadingOverlay();
        });
    }

    document.querySelectorAll(".pagination a").forEach(function (link) {

        link.addEventListener("click", function () {

            if (!link.closest(".disabled")) {
                mostrarLoadingOverlay();
            }

        });

    });

    // Para páginas nuevas
    document.querySelectorAll("[data-show-overlay]").forEach(function (element) {

        element.addEventListener("click", function () {

            mostrarLoadingOverlay();

        });

    });

});

window.addEventListener("pageshow", function () {

    ocultarLoadingOverlay();

});