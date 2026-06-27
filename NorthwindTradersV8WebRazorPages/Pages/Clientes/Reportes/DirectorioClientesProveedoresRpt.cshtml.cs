using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes.Reportes
{
    public class DirectorioClientesProveedoresRptModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        //public override string PageName => "/Clientes/Reportes/DirectorioClientesProveedoresRpt";
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
                    return "DirectorioClientesProveedores";

                if (MostrarClientes)
                    return "DirectorioClientes";

                if (MostrarProveedores)
                    return "DirectorioProveedores";

                return "DirectorioClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Reporte directorio de clientes y proveedores";

                if (MostrarClientes)
                    return "Reporte directorio de clientes";

                if (MostrarProveedores)
                    return "Reporte directorio de proveedores";

                return "Reporte directorio de clientes y proveedores";
            }
        }
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public DirectorioClientesProveedoresRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }

        public void OnGet()
        {
            // Primera carga de la página
            if (!Buscar)
                return;
            // Debe seleccionar al menos una opción
            if (!MostrarClientes && !MostrarProveedores)
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
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
                "RptClientesyProveedoresDirectorio.rdlc");

            var clientesProveedores = clienteBLL.ObtenerClientesProveedoresRpt(Tipo);

            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", clientesProveedores));

            return reporte;
        }
    }
}
