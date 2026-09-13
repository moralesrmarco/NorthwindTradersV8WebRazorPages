using NorthwindTradersV8WebRazorPages.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Reportes
{
    public class VentasAnualesModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public List<OpcionAños> Años { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Years { get; set; }

        public VentasAnualesModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            int totalAñosDisponibles =
                graficasBLL.ObtenerTotalAñosConVentas();

            int limite =
                Math.Min(totalAñosDisponibles, 10);

            for (int i = 2; i <= limite; i++)
            {
                Años.Add(new OpcionAños
                {
                    Texto = $"{i} Años",
                    Valor = i
                });
            }

            if (Years == 0 && Años.Count > 0)
            {
                Years = Años[0].Valor;
            }
        }

        public IActionResult OnGetVerPdf()
        {
            try
            {
                if (Years < 2)
                {
                    Years = 2;
                }

                var ventas =
                    graficasBLL.ObtenerVentasMensualesPorAños(Years);

                var dt = new DataTable();

                dt.Columns.Add("Mes", typeof(int));
                dt.Columns.Add("NombreMes", typeof(string));
                dt.Columns.Add("Año", typeof(int));
                dt.Columns.Add("Total", typeof(decimal));

                foreach (var v in ventas)
                {
                    dt.Rows.Add(
                        v.Mes,
                        v.NombreMes,
                        v.Year,
                        v.Total);
                }

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Pages",
                    "Graficas",
                    "Reportes",
                    "RptGraficaVentasAnuales.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter(
                        "Anio",
                        $"{Years} Años"),

                    new ReportParameter(
                        "Subtitulo",
                        $"Comparativo de ventas anuales de los últimos {Years} años")
                });

                byte[] pdf = report.Render("PDF");

                return File(
                    pdf,
                    "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class OpcionAños
        {
            public string Texto { get; set; } = string.Empty;
            public int Valor { get; set; }
        }
    }
}