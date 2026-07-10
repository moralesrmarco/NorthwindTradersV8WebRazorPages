using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias.Consultas
{
    public class ListadoCategoriasConProductosModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        public List<CategoriasConProductosDto> CategoriasConProductos { get; set; } = new();
        public ListadoCategoriasConProductosModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet()
        {
            CategoriasConProductos = categoriaBLL.ObtenerCategoriasConProductosRpt();
        }
    }
}
