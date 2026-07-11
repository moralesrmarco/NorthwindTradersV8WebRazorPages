using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos.Reportes
{
    public class ProductosAlfabeticoRptModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        public ProductosAlfabeticoRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet()
        {
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
                "ProductosAlfabetico.xlsx");
        }

        public IActionResult OnGetWord()
        {
            var reporte = CrearReporte();
            return File(
                reporte.Render("WORDOPENXML"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "ProductosAlfabetico.docx");
        }

        private LocalReport CrearReporte()
        {
            string titulo = "» Reporte de productos en orden alfabético «";
            string subtitulo = $" Ordenado por: [ Producto ] [ Ascendente ]";
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Pages",
                "Productos",
                "Reportes",
                "RptProductos.rdlc");
            var productos = productoBLL.ObtenerProductosAlfabeticoRpt();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", productos));
            ReportParameter rp = new ReportParameter("titulo", titulo);
            ReportParameter rp2 = new ReportParameter("subtitulo", subtitulo);
            reporte.SetParameters(new ReportParameter[] { rp, rp2 });
            return reporte;
        }
    }
}
