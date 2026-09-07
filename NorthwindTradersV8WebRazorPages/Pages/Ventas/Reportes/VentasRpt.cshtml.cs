using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas.Reportes
{
    public class VentasRptModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        [BindProperty(SupportsGet = true)]
        public VentasBuscarDto Filtro { get; set; } = new VentasBuscarDto();
        private string subtitulo = "";
        public VentasRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString);
        }
        public void OnGet()
        {
        }
        public IActionResult OnGetVerPdf()
        {
            var reporte = CrearReporte();

            return File(
                reporte.Render("PDF"),
                "application/pdf");
        }

        public IActionResult OnGetExcel()
        {
            var reporte = CrearReporte();

            return File(
                reporte.Render("EXCELOPENXML"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Ventas.xlsx");
        }

        public IActionResult OnGetWord()
        {
            var reporte = CrearReporte();

            return File(
                reporte.Render("WORDOPENXML"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Ventas.docx");
        }
        private LocalReport CrearReporte()
        {
            var criterios = ConstruirCriteriosBusqueda();

            var ventas = ventaBLL.ObtenerVentasRpt(
                true,
                criterios);

            ConstruirTituloSubtitulo();

            LocalReport reporte = new();

            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Pages",
                "Ventas",
                "Reportes",
                "RptVentasPorDiferentesCriterios.rdlc");

            reporte.DataSources.Clear();

            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    ventas));

            reporte.SetParameters(new[]
            {
                new ReportParameter("subtitulo", subtitulo)
            });

            reporte.SubreportProcessing +=
                OrderDetailsSubReportProcessing;

            return reporte;
        }
        private void OrderDetailsSubReportProcessing(
                   object sender,
                   SubreportProcessingEventArgs e)
        {
            int orderID = int.Parse(
                e.Parameters["OrderID"].Values[0]);

            var ventaDetalles =
                ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderID);

            e.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    ventaDetalles));
        }
        private VentasBuscarDto ConstruirCriteriosBusqueda()
        {
            var criterios = new VentasBuscarDto
            {
                IdIni = Filtro.IdIni,
                IdFin = Filtro.IdFin,

                Cliente = Filtro.Cliente?.Trim(),

                FVenta =
                    Filtro.FVentaIni.HasValue &&
                    Filtro.FVentaFin.HasValue,

                FVentaNull = Filtro.FVentaNull,

                FVentaIni = Filtro.FVentaIni?.Date,

                FVentaFin = Filtro.FVentaFin.HasValue
                    ? Filtro.FVentaFin.Value.Date.AddDays(1)
                    : null,

                FRequerido =
                    Filtro.FRequeridoIni.HasValue &&
                    Filtro.FRequeridoFin.HasValue,

                FRequeridoNull = Filtro.FRequeridoNull,

                FRequeridoIni = Filtro.FRequeridoIni?.Date,

                FRequeridoFin = Filtro.FRequeridoFin.HasValue
                    ? Filtro.FRequeridoFin.Value.Date.AddDays(1)
                    : null,

                FEnvio =
                    Filtro.FEnvioIni.HasValue &&
                    Filtro.FEnvioFin.HasValue,

                FEnvioNull = Filtro.FEnvioNull,

                FEnvioIni = Filtro.FEnvioIni?.Date,

                FEnvioFin = Filtro.FEnvioFin.HasValue
                    ? Filtro.FEnvioFin.Value.Date.AddDays(1)
                    : null,

                Empleado = Filtro.Empleado?.Trim(),
                CompañiaT = Filtro.CompañiaT?.Trim(),
                DirigidoA = Filtro.DirigidoA?.Trim()
            };

            // Si solo se proporciona el ID inicial,
            // usar el mismo valor como ID final.
            if (criterios.IdIni.HasValue &&
                !criterios.IdFin.HasValue)
            {
                criterios.IdFin = criterios.IdIni;
            }

            // Si solo se proporciona el ID final,
            // usar el mismo valor como ID inicial.
            if (!criterios.IdIni.HasValue &&
                criterios.IdFin.HasValue)
            {
                criterios.IdIni = criterios.IdFin;
            }

            return criterios;
        }
        private void ConstruirTituloSubtitulo()
        {
            subtitulo = "";

            if (Filtro.IdIni.HasValue)
            {
                subtitulo +=
                    $"[Id inicial: {Filtro.IdIni.Value:N0}] ";
            }

            if (Filtro.IdFin.HasValue)
            {
                subtitulo +=
                    $"[Id final: {Filtro.IdFin.Value:N0}] ";
            }

            if (!string.IsNullOrWhiteSpace(Filtro.Cliente))
            {
                subtitulo +=
                    $"[Cliente: %{Filtro.Cliente.Trim()}%] ";
            }

            // =============================================
            // FECHA DE VENTA
            // =============================================
            if (Filtro.FVentaIni.HasValue &&
                Filtro.FVentaFin.HasValue)
            {
                subtitulo +=
                    $"[Fecha de venta inicial: {Filtro.FVentaIni.Value.ToShortDateString()}] - " +
                    $"[Fecha de venta final: {Filtro.FVentaFin.Value.ToShortDateString()}] ";
            }

            if (Filtro.FVentaNull)
            {
                subtitulo +=
                    "[Fecha de venta inicial: Nulo] - " +
                    "[Fecha de venta final: Nulo] ";
            }

            // =============================================
            // FECHA REQUERIDO
            // =============================================
            if (Filtro.FRequeridoIni.HasValue &&
                Filtro.FRequeridoFin.HasValue)
            {
                subtitulo +=
                    $"[Fecha requerido inicial: {Filtro.FRequeridoIni.Value.ToShortDateString()}] - " +
                    $"[Fecha requerido final: {Filtro.FRequeridoFin.Value.ToShortDateString()}] ";
            }

            if (Filtro.FRequeridoNull)
            {
                subtitulo +=
                    "[Fecha requerido inicial: Nulo] - " +
                    "[Fecha requerido final: Nulo] ";
            }

            // =============================================
            // FECHA DE ENVÍO
            // =============================================
            if (Filtro.FEnvioIni.HasValue &&
                Filtro.FEnvioFin.HasValue)
            {
                subtitulo +=
                    $"[Fecha de envío inicial: {Filtro.FEnvioIni.Value.ToShortDateString()}] - " +
                    $"[Fecha de envío final: {Filtro.FEnvioFin.Value.ToShortDateString()}] ";
            }

            if (Filtro.FEnvioNull)
            {
                subtitulo +=
                    "[Fecha de envío inicial: Nulo] - " +
                    "[Fecha de envío final: Nulo] ";
            }

            if (!string.IsNullOrWhiteSpace(Filtro.Empleado))
            {
                subtitulo +=
                    $"[Vendedor: %{Filtro.Empleado.Trim()}%] ";
            }

            if (!string.IsNullOrWhiteSpace(Filtro.CompañiaT))
            {
                subtitulo +=
                    $"[Transportista: %{Filtro.CompañiaT.Trim()}%] ";
            }

            if (!string.IsNullOrWhiteSpace(Filtro.DirigidoA))
            {
                subtitulo +=
                    $"[Enviar a: %{Filtro.DirigidoA.Trim()}%]";
            }

            if (string.IsNullOrWhiteSpace(subtitulo))
            {
                subtitulo =
                    "Ningún criterio de selección fue especificado " +
                    "( incluye todos los registros de ventas )";
            }
        }
    }
}
