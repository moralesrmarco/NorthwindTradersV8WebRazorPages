using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly ProductoBLL _productoBLL;
        public DataTable Productos { get; set; }
        public IndexModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection");
            _productoBLL = new ProductoBLL(connectionString);
        }
        public void OnGet()
        {
            Productos = _productoBLL.ObtenerProductos();
        }
    }
}
