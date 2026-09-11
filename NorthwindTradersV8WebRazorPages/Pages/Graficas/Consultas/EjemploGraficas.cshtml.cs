using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class EjemploGraficasModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public EjemploGraficasModel(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");

            graficasBLL = new GraficasBLL(connectionString);
        }

        public IActionResult OnGetVentasMensuales()
        {
            var datos = graficasBLL.ObtenerVentasMensuales(1997);

            return new JsonResult(datos.Select(x => new
            {
                nombreMes = x.NombreMes,
                total = x.Total
            }));
        }
    }
}