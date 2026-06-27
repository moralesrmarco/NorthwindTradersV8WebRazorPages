using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes.Reportes
{
    public class DirectorioPorCiudadClientesProveedoresRptModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        private readonly ClienteService clienteService;
        [BindProperty(SupportsGet = true)]
        public bool MostrarClientes { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool MostrarProveedores { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool Buscar { get; set; }
        public string Tipo
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "DirectorioPorCiudadClientesProveedores";

                if (MostrarClientes)
                    return "DirectorioPorCiudadClientes";

                if (MostrarProveedores)
                    return "DirectorioPorCiudadProveedores";

                return "DirectorioPorCiudadClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Reporte directorio de clientes y proveedores por ciudad";

                if (MostrarClientes)
                    return "Reporte directorio de clientes por ciudad";

                if (MostrarProveedores)
                    return "Reporte directorio de proveedores por ciudad";

                return "Reporte directorio de clientes y proveedores por ciudad";
            }
        }
        [BindProperty(SupportsGet = true)]
        public string? CiudadPaisSeleccionado { get; set; }
        public List<SelectListItem> CiudadesPaises { get; set; } = [];
        // Propiedades para la vista
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public string TituloReporte
        {
            get
            {
                string tipo = (MostrarClientes, MostrarProveedores) switch
                {
                    (true, true) => "Reporte directorio de clientes y proveedores por ciudad",
                    (true, false) => "Reporte directorio de clientes por ciudad",
                    (false, true) => "Reporte directorio de proveedores por ciudad",
                    _ => "Reporte directorio de clientes y proveedores por ciudad"
                };

                string ciudad = CiudadPaisSeleccionado == "00000"
                    ? "Todas las ciudades"
                    : $"Ciudad: {CiudadPaisSeleccionado}";

                return $"» {tipo} [ {ciudad} ] «";
            }
        }
        public DirectorioPorCiudadClientesProveedoresRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            clienteService = new ClienteService(connectionString);
        }

        public void OnGet()
        {
            LlenarCombo();
            // Primera carga de la página
            if (!Buscar)
                return;
            // Debe seleccionar al menos una opción
            if (string.IsNullOrWhiteSpace(CiudadPaisSeleccionado) || (!MostrarClientes && !MostrarProveedores))
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                Buscar = false;
            }
        }
        public IActionResult OnGetVerPdf()
        {
            var reporte = CrearReporte();
            return File(reporte.Render("PDF"), "application/pdf");
        }

        public IActionResult OnGetExcel()
        {
            var reporte = CrearReporte();
            return File(
                reporte.Render("EXCELOPENXML"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioClientesProveedores.xlsx");
        }

        public IActionResult OnGetWord()
        {
            var reporte = CrearReporte();
            return File(
                reporte.Render("WORDOPENXML"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioClientesProveedores.docx");
        }

        private LocalReport CrearReporte()
        {
            LocalReport reporte = new();

            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Pages",
                "Clientes",
                "Reportes",
                "RptClientesyProveedoresDirectorioxCiudad.rdlc");

            var clientesProveedores = clienteBLL.ObtenerClientesProveedoresPorCiudadRpt(Tipo, CiudadPaisSeleccionado);

            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", clientesProveedores));
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", TituloReporte)
            });
            return reporte;
        }
        private void LlenarCombo()
        {
            CiudadesPaises = clienteService
                    .ObtenerCiudadesPaisesVwCliProvCbo()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Key,
                        Value = x.Value
                    })
                    .ToList();
        }
    }
}
