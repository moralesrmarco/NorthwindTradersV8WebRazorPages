using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class InsertarModel : PageModel
    {
        private readonly ProductoBLL _productoBLL;
        [BindProperty]
        public Producto Producto { get; set; } = new Producto();
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _productoBLL = new ProductoBLL(connectionString);
        }
        public IActionResult OnPost()
        {
            if (Producto != null)
            {
                var resultado = _productoBLL.Insertar(Producto);
                if (resultado.Exito)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>El producto <strong>{Producto.ProductName}</strong>:</p>{resultado.Mensaje}";
                }
            }
            return Page();
        }
    }
}
