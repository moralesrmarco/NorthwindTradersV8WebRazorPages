using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas.Reportes
{
    public class NotaRemisionRptModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        [BindProperty(SupportsGet = true)]
        public int orderId { get; set; }
        public NotaRemisionRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString);
        }
        public IActionResult OnGetPdf()
        {
            if (orderId <= 0)
                return BadRequest("OrderId inválido.");
            string strFecha = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
            LocalReport report = new LocalReport();
            string rutaReporte =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Pages",
                    "Ventas",
                    "Reportes",
                    "RptNotaRemision9.rdlc");
            using (var stream = System.IO.File.OpenRead(rutaReporte))
            {
                report.LoadReportDefinition(stream);
            }
            DataTable dtDummy = new DataTable("DataSetDummy");
            dtDummy.Columns.Add("Dummy_", typeof(int));
            dtDummy.Rows.Add(1);
            var venta = ventaBLL.ObtenerVentaPorIdDt(orderId);
            if (venta == null)
                throw new Exception($"No se encontró la venta con OrderID = {orderId}");
            var detalle = ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderId);
            report.DataSources.Add(
                new ReportDataSource("DataSetDummy", dtDummy));
            report.DataSources.Add(
                new ReportDataSource("DataSetVenta", venta));
            report.DataSources.Add(
                new ReportDataSource("DataSet1", detalle));
            report.SetParameters(new[]
            {
                new ReportParameter("PedidoId", orderId.ToString()),
                new ReportParameter("FechaHora", $"Fecha: {strFecha}"),
                new ReportParameter("Para", "Para: Cliente.")
            });
            byte[] pdf = report.Render("PDF");
            return File(pdf, "application/pdf");
        }
    }
}
