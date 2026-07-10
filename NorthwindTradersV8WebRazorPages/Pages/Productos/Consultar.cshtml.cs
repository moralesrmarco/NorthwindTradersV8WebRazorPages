using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class ConsultarModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        
        [BindProperty]
        public Producto? Producto { get; set; } = new Producto();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var producto = productoBLL.ObtenerProductoPorId(id);
            if (producto == null)
                TempData["Error"] = "<p>Producto no encontrado</p>" + StringsCommons.Nefep;
            else
                Producto = producto;
            return Page();
        }
    }
}
