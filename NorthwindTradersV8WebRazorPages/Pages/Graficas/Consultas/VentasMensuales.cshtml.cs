using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class VentasMensualesModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;
        public DataTable Años { get; set; } = new DataTable();
        public int AñoActual { get; set; }
        public VentasMensualesModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.graficasBLL = new GraficasBLL(connectionString);
        }
        public void OnGet()
        {
            Años = graficasBLL.ObtenerTop10AñosDeVentas(false);
            AñoActual = DateTime.Now.Year;
        }
        public IActionResult OnGetVentasMensuales(int year)
        {
            var datos = graficasBLL.ObtenerVentasMensuales(year);
            return new JsonResult(datos);
        }
    }
}
