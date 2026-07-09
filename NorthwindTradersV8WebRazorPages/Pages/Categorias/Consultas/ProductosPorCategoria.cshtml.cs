using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias.Consultas
{
    public class ProductosPorCategoriaModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        [BindProperty]
        public Categoria? Categoria { get; set; } = new Categoria();
        public List<Producto> Productos { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public ProductosPorCategoriaModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id, string? returnUrl)
        {
            ReturnUrl = returnUrl;
            var categoria = categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
            else
            {
                Categoria = categoria;
                Productos = categoriaBLL.ObtenerProductosPorCategoriaId(categoria.CategoryID);
            }
            return Page();
        }
    }
}
