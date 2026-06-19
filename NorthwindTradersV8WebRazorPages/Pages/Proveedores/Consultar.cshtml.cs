using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class ConsultarModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        [BindProperty]
        public Proveedor? Proveedor { get; set; } = new Proveedor();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            proveedorBLL = new ProveedorBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(string id)
        {
            var proveedor = proveedorBLL.ObtenerProveedorPorId(id);
            if (proveedor == null)
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
            else
                Proveedor = proveedor;
            return Page();
        }
    }
}
