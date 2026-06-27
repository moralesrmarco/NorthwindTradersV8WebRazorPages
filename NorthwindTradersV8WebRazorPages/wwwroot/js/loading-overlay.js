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
    // Todos los formularios
    document.querySelectorAll("form").forEach(function (formulario) {
        formulario.addEventListener("submit", function () {
            mostrarLoadingOverlay();
        });
    });
    // Paginación
    document.querySelectorAll(".pagination a").forEach(function (link) {

        link.addEventListener("click", function () {

            if (!link.closest(".disabled")) {
                mostrarLoadingOverlay();
            }

        });
    });
    // Botones o enlaces marcados    
    document.querySelectorAll("[data-show-overlay]").forEach(function (element) {
        element.addEventListener("click", function () {
            mostrarLoadingOverlay();
        });
    });
    // Reportes PDF
    document.querySelectorAll("iframe[data-report-src]").forEach(function (iframe) {

        mostrarLoadingOverlay();

        iframe.addEventListener("load", function () {

            ocultarLoadingOverlay();

        });

        iframe.src = iframe.dataset.reportSrc;

    });
});

window.addEventListener("pageshow", function () {
    ocultarLoadingOverlay();
});