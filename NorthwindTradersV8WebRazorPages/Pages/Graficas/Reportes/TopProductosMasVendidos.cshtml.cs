using NorthwindTradersV8WebRazorPages.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Reportes
{
    public class TopProductosMasVendidosModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public DataTable Años { get; set; } = new();

        public List<OpcionProductos> OpcionesProductos { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int TopProductos { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }

        public TopProductosMasVendidosModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            for (int i = 10; i <= 50; i += 5)
            {
                OpcionesProductos.Add(new OpcionProductos
                {
                    Texto = $"{i} productos",
                    Valor = i
                });
            }

            Años =
                graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (TopProductos == 0)
                TopProductos = 10;

            if (Anio == 0)
                Anio = DateTime.Today.Year;
        }

        public IActionResult OnGetVerPdf()
        {
            try
            {
                if (TopProductos < 10)
                    TopProductos = 10;

                if (Anio == 0)
                    Anio = DateTime.Today.Year;

                DataTable dt =
                    graficasBLL.ObtenerTopProductosRpt(
                        TopProductos,
                        Anio);

                string textoAnio;

                if (Anio == -1)
                    textoAnio = "Todos los años";
                else
                    textoAnio = Anio.ToString();

                string titulo =
                    $"» Reporte gráfico top {TopProductos} productos más vendidos ({textoAnio}) «";

                string subtitulo =
                    $"Top {TopProductos} productos más vendidos ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Pages",
                    "Graficas",
                    "Reportes",
                    "RptGraficaTopProductosMásVendidos.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Titulo", titulo),
                    new ReportParameter("Subtitulo", subtitulo)
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

        public class OpcionProductos
        {
            public string Texto { get; set; } = string.Empty;
            public int Valor { get; set; }
        }
    }
}