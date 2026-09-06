using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas.Reportes
{
    public class VentasRptModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        public List<VentaRptDto> Ventas { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public VentasBuscarDto Filtro { get; set; } = new VentasBuscarDto();
        public bool SeBusco { get; set; }
        public VentasRptModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString);
        }
        public IActionResult OnGetVerPdf()
        {
            var criterios = new VentasBuscarDto
            {
                IdIni = Filtro.IdIni,
                IdFin = Filtro.IdFin,
                Cliente = Filtro.Cliente?.Trim(),

                FVenta = Filtro.FVentaIni.HasValue &&
                         Filtro.FVentaFin.HasValue,

                FVentaIni = Filtro.FVentaIni?.Date,

                FVentaFin = Filtro.FVentaFin?
                    .Date
                    .AddDays(1),

                FVentaNull = Filtro.FVentaNull,

                FRequerido = Filtro.FRequeridoIni.HasValue &&
                             Filtro.FRequeridoFin.HasValue,

                FRequeridoIni = Filtro.FRequeridoIni?.Date,

                FRequeridoFin = Filtro.FRequeridoFin?
                    .Date
                    .AddDays(1),

                FRequeridoNull = Filtro.FRequeridoNull,

                FEnvio = Filtro.FEnvioIni.HasValue &&
                         Filtro.FEnvioFin.HasValue,

                FEnvioIni = Filtro.FEnvioIni?.Date,

                FEnvioFin = Filtro.FEnvioFin?
                    .Date
                    .AddDays(1),

                FEnvioNull = Filtro.FEnvioNull,

                Empleado = Filtro.Empleado?.Trim(),

                CompañiaT = Filtro.CompañiaT?.Trim(),

                DirigidoA = Filtro.DirigidoA?.Trim()
            };

            //var ventas = ventaBLL.ObtenerVentasRpt(
            //    true,
            //    criterios);

            // Generar PDF aquí

            return Page();
        }
    }
}
