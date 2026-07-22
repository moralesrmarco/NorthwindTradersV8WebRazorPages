using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class ConsultarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        [BindProperty]
        public VentaDto? Venta { get; set; } = new VentaDto();
        [BindProperty]
        public List<VentaDetalle> VentaDetalle { get; set; } = new List<VentaDetalle>();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        [BindProperty]
        public string PestanaActiva { get; set; } = "#consulta";
        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var venta = ventaBLL.ObtenerVentaPorId(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                return Page();
            }
            else
                Venta = venta;
            VentaDetalle = ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            CalcularTotales();
            return Page();
        }
        public void CalcularTotales()
        {
            Venta!.NumeroProductos = VentaDetalle.Count;
            Venta.TotalUnidades = VentaDetalle.Sum(d => d.Quantity);
            Venta.SubtotalImporte = VentaDetalle.Sum(d => d.SubtotalDelImporteConIVAIncluido);
            Venta.SubtotalImporteDescuento = VentaDetalle.Sum(d => d.SubtotalDelAhorroTotalDespuesDescuento);
            Venta.SubtotalImporteConDescuento = VentaDetalle.Sum(d => d.SubtotalDelImporteConIVAConDescuento);
            Venta.SubtotalImporteSinIVA = VentaDetalle.Sum(d => d.SubtotalDelImporteSinIVAConDescuento);
            Venta.SubtotalImporteConIVA = VentaDetalle.Sum(d => d.SubtotalIVADespuesDelDescuento);
            Venta.Total = VentaDetalle.Sum(d => d.Subtotal);
        }
    }
}
