using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos.Consultas
{
    public class ProductosPorEncimaDelPrecioPromedioModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        public decimal PrecioPromedio { get; set; }
        public DataTable Productos { get; set; } = new();
        public ProductosPorEncimaDelPrecioPromedioModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet()
        {
            //CalcularPrecioPromedio();
            ObtenerProductosPorEncimaDelPrecioPromedio();
        }
        private void CalcularPrecioPromedio()
        {
            PrecioPromedio = productoBLL.ObtenerPrecioPromedio();
        }
        private void ObtenerProductosPorEncimaDelPrecioPromedio()
        {
            Productos = productoBLL.ObtenerProductosPorEncimaDelPrecioPromedio();
        }
    }
}
