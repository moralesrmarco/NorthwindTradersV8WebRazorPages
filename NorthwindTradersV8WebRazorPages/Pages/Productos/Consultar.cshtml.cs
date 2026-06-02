using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class ConsultarModel : PageModel
    {
        private readonly ProductoBLL _productoBLL;
        
        [BindProperty]
        public Producto Producto { get; set; } = new Producto();

        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found"); 
            _productoBLL = new ProductoBLL(connectionString);
        }
        public void OnGet(int id)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);
            if (producto != null)
            {
                Producto = producto;
            }
        }
    }
}
