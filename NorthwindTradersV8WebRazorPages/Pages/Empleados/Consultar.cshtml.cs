using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class ConsultarModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        [BindProperty]
        public Empleado? Empleado { get; set; } = new Empleado();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            empleadoBLL = new EmpleadoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var empleado = empleadoBLL.ObtenerEmpleadoPorId(id);
            if (empleado == null)
                TempData["Error"] = "<p>Empleado no encontrado</p>" + StringsCommons.Nefep;
            else
                Empleado = empleado;
            return Page();
        }

        // 🔹 Nuevo método para generar el reporte en PDF
        public IActionResult OnGetReporte(int id)
        {
            var empleado = empleadoBLL.ObtenerEmpleadoPorIdRptDto(id);
            if (empleado == null)
                return NotFound();

            // Construir la ruta al archivo físico publicado
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(),
                                             "Pages", "Empleados", "Reportes", "RptEmpleado.rdlc");

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            localReport.DataSources.Add(new ReportDataSource("DataSet1", new List<EmpleadoRptDto> { empleado }));

            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Warning[] warnings;

            byte[] pdfBytes = localReport.Render(
                "PDF", null, out mimeType, out encoding,
                out fileNameExtension, out streams, out warnings);

            return new FileStreamResult(new MemoryStream(pdfBytes), "application/pdf");
        }
    }
}
