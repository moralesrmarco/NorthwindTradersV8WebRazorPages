using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos.Consultas
{
    public class ListadoAlfabeticoProductosModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        public ProductosBuscarDto Filtro { get; set; }
        public List<ProductoDto> Productos { get; set; } = new();
        public ListadoAlfabeticoProductosModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            Filtro = new ProductosBuscarDto();
        }
        public void OnGet()
        {
            Filtro = new ProductosBuscarDto
            {
                IdIni = null,
                IdFin = null,
                Producto = string.Empty,
                Categoria = 0,
                Proveedor = 0,
                OrdenadoPor = "ProductName",
                AscDesc = "ASC"
            };
            Productos = ObtenerDatos();
        }
        private List<ProductoDto> ObtenerDatos()
        {
            return productoBLL.ObtenerProductosRpt(Filtro);
        }
    }
}
