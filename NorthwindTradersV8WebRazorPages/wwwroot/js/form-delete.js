function configurarEliminar(btnId) {

    const btn = document.getElementById(btnId);

    if (!btn) {
        return true;
    }

    if (btn.disabled) {
        return false;
    }

    btn.disabled = true;

    btn.innerHTML =
        '<span class="spinner-border spinner-border-sm me-1"></span> Procesando...';

    const overlay = document.getElementById('loadingOverlay');

    if (overlay) {
        overlay.style.display = 'block';
    }

    return true;
}