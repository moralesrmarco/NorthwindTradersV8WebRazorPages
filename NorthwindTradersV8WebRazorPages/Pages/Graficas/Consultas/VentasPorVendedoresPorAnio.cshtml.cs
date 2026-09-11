using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class VentasPorVendedoresPorAnioModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public int AñoInicial { get; set; }
        public DataTable Años { get; set; } = new DataTable();
        public VentasPorVendedoresPorAnioModel(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not found");

            this.graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            Años = graficasBLL.ObtenerTop10AñosDeVentas(false);
            AñoInicial = DateTime.Today.Year;
        }

        public IActionResult OnGetVentasPorVendedores(int anio)
        {
            var datos =
                graficasBLL.ObtenerVentasPorVendedores(anio);

            var vendedores = new List<object>();

            decimal totalVentas = 0;

            foreach (var dato in datos)
            {
                totalVentas += dato.TotalVentas;

                vendedores.Add(new
                {
                    vendedor = dato.Vendedor,
                    totalVentas = dato.TotalVentas
                });
            }

            return new JsonResult(new
            {
                vendedores,
                totalVentas
            });
        }
    }
}