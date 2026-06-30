using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores.Consultas
{
    public class ProductosPorProveedorModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        [BindProperty]
        public Proveedor? Proveedor { get; set; } = new Proveedor();
        public List<ProductosPorProveedorDto> ProductosPorProveedor { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public ProductosPorProveedorModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            proveedorBLL = new ProveedorBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(string id, string? returnUrl)
        {
            ReturnUrl = returnUrl;
            var proveedor = proveedorBLL.ObtenerProveedorPorId(id);
            if (proveedor == null)
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
            else
            {
                Proveedor = proveedor;
                ProductosPorProveedor = proveedorBLL.ObtenerProductosPorProveedorId(proveedor.SupplierID);
            }
            return Page();
        }
    }
}
