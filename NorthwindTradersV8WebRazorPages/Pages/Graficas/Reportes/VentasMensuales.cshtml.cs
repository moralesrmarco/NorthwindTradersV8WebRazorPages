using NorthwindTradersV8WebRazorPages.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Reportes
{
    public class VentasMensualesModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;
        public DataTable Años { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;

        public VentasMensualesModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            try
            {
                Años = graficasBLL.ObtenerTop10AñosDeVentas(false);

                if (Anio == 0)
                    Anio = DateTime.Today.Year;

                if (Anio > 0)
                {
                    Titulo =
                        $"» Reporte gráfico de ventas mensuales ({Anio}) «";

                    Subtitulo =
                        $"Ventas mensuales ({Anio})";
                }
                else
                {
                    Titulo =
                        "» Reporte gráfico de ventas mensuales (todos los años) «";

                    Subtitulo =
                        "Ventas mensuales (todos los años)";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        public IActionResult OnGetVerPdf()
        {
            try
            {
                if (Anio == 0)
                    Anio = DateTime.Today.Year;

                var ventas = graficasBLL.ObtenerVentasMensuales(Anio);

                var dt = new DataTable();

                dt.Columns.Add("Mes", typeof(int));
                dt.Columns.Add("Total", typeof(decimal));
                dt.Columns.Add("NombreMes", typeof(string));

                foreach (var v in ventas)
                {
                    dt.Rows.Add(
                        v.Mes,
                        v.Total,
                        v.NombreMes);
                }

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Pages",
                    "Graficas",
                    "Reportes",
                    "RptGraficaVentasMensuales.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                string anioParametro =
                    Anio > 0
                        ? $"({Anio})"
                        : "(todos los años)";

                string subtitulo =
                    Anio > 0
                        ? $"Ventas mensuales ({Anio})"
                        : "Ventas mensuales (todos los años)";

                report.SetParameters(new[]
                {
            new ReportParameter("Anio", anioParametro),
            new ReportParameter("Subtitulo", subtitulo)
        });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}