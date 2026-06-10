using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados.Reportes
{
    public class RptEmpleadosConFotoModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;

        public RptEmpleadosConFotoModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.empleadoBLL = new EmpleadoBLL(connectionString);
        }
        public void OnGet()
        {
        }
        public IActionResult OnGetVerPdf()
        {
            LocalReport reporte = new();

            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Pages", "Empleados", "Reportes", "RptEmpleadosConFoto.rdlc");

            var empleados = empleadoBLL.ObtenerTodosLosEmpleados();

            reporte.DataSources.Clear();

            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));

            string mimeType, encoding, extension;
            string[] streams;
            Warning[] warnings;

            byte[] pdfBytes = reporte.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out extension,
                out streams,
                out warnings);

            return new FileStreamResult(
                new MemoryStream(pdfBytes),
                "application/pdf");
        }

        public IActionResult OnGetExcel()
        {
            return GenerarReporte("EXCELOPENXML",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Empleados.xlsx");
        }

        public IActionResult OnGetWord()
        {
            return GenerarReporte("WORDOPENXML",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Empleados.docx");
        }

        private FileContentResult GenerarReporte(
            string formato,
            string contentType,
            string nombreArchivo)
        {
            LocalReport reporte = new();
            reporte.ReportPath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                        "Pages", "Empleados", "Reportes", "RptEmpleadosConFoto.rdlc");
            var empleados = empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));

            byte[] bytes = reporte.Render(formato);

            return File(bytes, contentType, nombreArchivo);
        }
    }
}
