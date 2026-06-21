function configurarSubmitFormulario(campoValidacion) {

    $('form').on('submit', function () {


        const btnGuardar = document.getElementById('btnGuardar');

        if (btnGuardar && btnGuardar.disabled) {
            return false;
        }

        if (!$(this).valid()) {
            return false;
        }

        if (!validarPais(campoValidacion)) {
            return false;
        }

        asegurarValorPais();

        if (btnGuardar) {

            btnGuardar.disabled = true;

            btnGuardar.innerHTML =
                '<span class="spinner-border spinner-border-sm me-1"></span> Procesando...';
        }

        mostrarOverlay();

        return true;
    });

    $('.btn-cancelar').on('click', function () {
        mostrarOverlay();
    });


    window.addEventListener('pageshow', function () {

        const overlay = document.getElementById('loadingOverlay');

        if (overlay) {
            overlay.style.display = 'none';
        }
    });
}

function configurarPais(campoValidacion) {

    $('#cboPais').select2({
        tags: true,
        placeholder: 'Seleccione o escriba un país',
        width: '100%'
    });

    $('#cboPais').on('select2:closing', function () {

        const texto = $('.select2-search__field').val();

        if (!texto) {
            return;
        }

        const existe =
            $('#cboPais option').filter(function () {
                return $(this).val() === texto;
            }).length > 0;

        if (!existe) {
            $('#cboPais').append(
                new Option(texto, texto, true, true)
            );
        }

        $('#cboPais').val(texto).trigger('change');
    });

    $('#cboPais').on('change', function () {
        $(`[data-valmsg-for="${campoValidacion}"]`).text('');
    });
}

function validarPais(campoValidacion) {

    const pais = $('#cboPais').val();
    if (!pais || pais.trim() === '') {

        $(`[data-valmsg-for="${campoValidacion}"]`)
            .text('Seleccione o escriba un país.');

        return false;
    }
    if (pais && pais.length > 15) {

        $(`[data-valmsg-for="${campoValidacion}"]`)
            .text('El país no puede exceder de 15 caracteres.');

        return false;
    }

    return true;
}
function asegurarValorPais() {

    const texto = $('.select2-search__field').val();

    if (!texto) {
        return;
    }

    const existe =
        $('#cboPais option').filter(function () {
            return $(this).val() === texto;
        }).length > 0;

    if (!existe) {
        $('#cboPais').append(
            new Option(texto, texto, true, true)
        );
    }

    $('#cboPais').val(texto).trigger('change');
}

function mostrarOverlay() {

    const overlay = document.getElementById('loadingOverlay');

    if (overlay) {
        overlay.style.display = 'block';
    }
}