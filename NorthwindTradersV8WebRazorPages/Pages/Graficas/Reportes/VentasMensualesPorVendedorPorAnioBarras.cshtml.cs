using NorthwindTradersV8WebRazorPages.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Reportes
{
    public class VentasMensualesPorVendedorPorAnioBarrasModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public DataTable Años { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }

        public VentasMensualesPorVendedorPorAnioBarrasModel(
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            Años =
                graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (Anio == 0)
                Anio = DateTime.Today.Year;
        }

        public IActionResult OnGetVerPdf()
        {
            try
            {
                if (Anio == 0)
                    Anio = DateTime.Today.Year;

                DataTable dt =
                    graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(Anio);

                string textoAnio =
                    Anio > 0
                        ? Anio.ToString()
                        : "todos los años";

                string subtitulo =
                    $"Ventas mensuales por vendedores ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Pages",
                    "Graficas",
                    "Reportes",
                    "RptGraficaVentasMensualesPorVendedorPorAnioBarras.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter(
                        "Subtitulo",
                        subtitulo),

                    new ReportParameter(
                        "Anio",
                        Anio.ToString())
                });

                byte[] pdf =
                    report.Render("PDF");

                return File(
                    pdf,
                    "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}