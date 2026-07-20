using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class BuscarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        public List<VentaDto> Ventas { get; set; } = new();
        public VentasBuscarDto Filtro { get; set; } = new VentasBuscarDto();
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
            Filtro = new VentasBuscarDto
            {
                IdIni = 0,
                IdFin = 0,
                Cliente = "",
                FVenta = false,
                FVentaIni = null,
                FVentaFin = null,
                FVentaNull = false,
                FRequerido = false,
                FRequeridoIni = null,
                FRequeridoFin = null,
                FRequeridoNull = false,
                FEnvio = false,
                FEnvioIni = null,
                FEnvioFin = null,
                FEnvioNull = false,
                Empleado = "",
                CompañiaT = "",
                DirigidoA = ""
            };
            Ventas = ObtenerDatos();
        }
        public List<VentaDto> ObtenerDatos()
        {
            //return ventaBLL.ObtenerVentas(Filtro);
            return new List<VentaDto>();
        }
    }
}
