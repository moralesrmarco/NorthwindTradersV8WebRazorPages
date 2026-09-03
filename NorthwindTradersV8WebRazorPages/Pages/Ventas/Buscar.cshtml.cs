using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class BuscarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        public List<VentaDto> Ventas { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public VentasBuscarDto Filtro { get; set; } = new VentasBuscarDto();
        public bool SeBusco { get; set; }
        public BuscarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);

        }
        public void OnGet()
        {
            Filtro.FVenta =
                Filtro.FVentaIni.HasValue ||
                Filtro.FVentaFin.HasValue;
            Filtro.FRequerido =
                Filtro.FRequeridoIni.HasValue ||
                Filtro.FRequeridoFin.HasValue;
            Filtro.FEnvio =
                Filtro.FEnvioIni.HasValue ||
                Filtro.FEnvioFin.HasValue;
            if (Filtro.IdIni.HasValue && Filtro.IdIni <= 0)
            {
                ModelState.AddModelError("Filtro.IdIni",
                    "El Id inicial debe ser mayor que cero");
            }
            if (Filtro.IdFin.HasValue && Filtro.IdFin <= 0)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor que cero");
            }
            if (Filtro.IdIni.HasValue &&
                Filtro.IdFin.HasValue &&
                Filtro.IdIni > Filtro.IdFin)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor o igual al Id inicial");
            }
            SeBusco = Request.Query.Count > 0;
            if (SeBusco && ModelState.IsValid)
            {
                Ventas = ventaBLL.BuscarVentas(Filtro);
            }
            else
                Ventas = new List<VentaDto>();
        }
    }
}
