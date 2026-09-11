using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class TopProductosMasVendidosModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public DataTable Años { get; set; } = new DataTable();

        public int CantidadInicial { get; set; } = 10;

        public int AñoInicial { get; set; } = -1;

        public TopProductosMasVendidosModel(IConfiguration configuration)
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
        }

        public IActionResult OnGetTopProductos(int cantidad, int anio)
        {
            var datos =
                graficasBLL.ObtenerTopProductos(cantidad, anio);

            var productos = new List<object>();

            int totalUnidades = 0;

            foreach (DataRow fila in datos.Rows)
            {
                string nombre =
                    Convert.ToString(fila["NombreProducto"]) ?? "";

                int cantidadVendida =
                    Convert.ToInt32(fila["CantidadVendida"]);

                totalUnidades += cantidadVendida;

                productos.Add(new
                {
                    nombreProducto = nombre,
                    cantidadVendida = cantidadVendida
                });
            }

            return new JsonResult(new
            {
                productos,
                totalUnidades
            });
        }
    }
}