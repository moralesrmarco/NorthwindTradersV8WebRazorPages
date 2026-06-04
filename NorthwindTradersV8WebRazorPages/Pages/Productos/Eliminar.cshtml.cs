using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class EliminarModel : PageModel
    {
        private readonly ProductoBLL _productoBLL;

        [BindProperty]
        public Producto? Producto { get; set; }

        [BindProperty]
        public string? CategoriaName { get; set; }

        [BindProperty]
        public string? ProveedorName { get; set; }

        public EliminarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _productoBLL = new ProductoBLL(connectionString);
        }
        public IActionResult OnGet(int id)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);

            if (producto == null)
                 TempData["Error"] = "<p>Producto no encontrado</p>" + StringsCommons.Nefep;
            else
               Producto = producto;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Producto != null)
            {
                var resultado = _productoBLL.Eliminar(Producto);
                if (resultado.Exito)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>El producto con Id: <strong>{Producto.ProductID}</strong> - Nombre de producto: <strong>{Producto.ProductName}</strong>:</p>{resultado.Mensaje}";
                }
                // 🔹 Reconstruir las propiedades de navegación con los valores que viajaron en los hidden inputs
                Producto.Categoria = new Categoria { CategoryName = CategoriaName };
                Producto.Proveedor = new Proveedor { CompanyName = ProveedorName };
            }
            return Page();
        }
    }
}
