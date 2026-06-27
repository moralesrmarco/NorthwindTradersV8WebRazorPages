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
    public class DirectorioPorPaisClientesProveedoresRptModel : PageModel
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
                    return "DirectorioPorPaisClientesProveedores";

                if (MostrarClientes)
                    return "DirectorioPorPaisClientes";

                if (MostrarProveedores)
                    return "DirectorioPorPaisProveedores";

                return "DirectorioPorPaisClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Reporte directorio de clientes y proveedores por país";

                if (MostrarClientes)
                    return "Reporte directorio de clientes por país";

                if (MostrarProveedores)
                    return "Reporte directorio de proveedores por país";

                return "Reporte directorio de clientes y proveedores por país";
            }
        }
        [BindProperty(SupportsGet = true)]
        public string? PaisSeleccionado { get; set; }
        public List<SelectListItem> Paises { get; set; } = [];
        // Propiedades para la vista
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public string TituloReporte
        {
            get
            {
                string tipo = (MostrarClientes, MostrarProveedores) switch
                {
                    (true, true) => "Reporte directorio de clientes y proveedores por país",
                    (true, false) => "Reporte directorio de clientes por país",
                    (false, true) => "Reporte directorio de proveedores por país",
                    _ => "Reporte directorio de clientes y proveedores por país"
                };

                string pais = PaisSeleccionado == "00000"
                    ? "Todos los paises"
                    : $"País: {PaisSeleccionado}";

                return $"» {tipo} [ {pais} ] «";
            }
        }
        public DirectorioPorPaisClientesProveedoresRptModel(IConfiguration configuration)
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
            if (string.IsNullOrWhiteSpace(PaisSeleccionado) || (!MostrarClientes && !MostrarProveedores))
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
                "RptClientesyProveedoresDirectorioxPais.rdlc");

            var clientesProveedores = clienteBLL.ObtenerClientesProveedoresPorPaisRpt(Tipo, PaisSeleccionado);

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
            Paises = clienteService
                    .ObtenerPaisesVwCliProvCbo()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Key,
                        Value = x.Value
                    })
                    .ToList();
        }
    }
}
