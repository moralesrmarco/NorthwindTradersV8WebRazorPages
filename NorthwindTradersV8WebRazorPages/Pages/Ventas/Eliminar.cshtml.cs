using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class EliminarModel : PageModel
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
        public bool BloquearEliminacion { get; set; }
        public EliminarModel(IConfiguration configuration)
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
            if (!CargarVenta(id))
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Venta != null)
            {
                var resultado = ventaBLL.Eliminar(Venta);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return LocalRedirect(ReturnUrl);
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>La venta con Id: <strong>{Venta.OrderID}</strong> del cliente: <strong>{Venta?.CustomerCompanyName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        BloquearEliminacion = true;
                    CargarVenta(Venta.OrderID);
                }
            }
            return Page();
        }
        private bool CargarVenta(int id)
        {
            Venta = ventaBLL.ObtenerVentaPorId(id);

            if (Venta == null)
                return false;

            VentaDetalle = ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            CalcularTotales();

            return true;
        }
        private void CalcularTotales()
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
