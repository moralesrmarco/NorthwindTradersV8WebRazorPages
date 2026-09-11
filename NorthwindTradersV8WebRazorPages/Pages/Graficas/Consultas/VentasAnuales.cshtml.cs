using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class VentasAnualesModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public int TotalAñosDisponibles { get; set; }

        public int AñosIniciales { get; set; } = 2;

        public VentasAnualesModel(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not found");

            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            TotalAñosDisponibles =
                graficasBLL.ObtenerTotalAñosConVentas();
        }

        public IActionResult OnGetVentasMensualesPorAños(int years)
        {
            var datos =
                graficasBLL.ObtenerVentasMensualesPorAños(years);

            return new JsonResult(datos);
        }
    }
}