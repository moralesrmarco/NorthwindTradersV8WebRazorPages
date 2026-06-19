using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class EliminarModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        [BindProperty]
        public Proveedor? Proveedor { get; set; } = new Proveedor();
        public bool BloquearEliminacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public EliminarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            proveedorBLL = new ProveedorBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(string id)
        {
            var cliente = proveedorBLL.ObtenerProveedorPorId(id);
            if (cliente == null)
            {
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
                BloquearEliminacion = true;
            }
            else
                Proveedor = cliente;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Proveedor != null)
            {
                var resultado = proveedorBLL.Eliminar(Proveedor);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return LocalRedirect(ReturnUrl);
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>El proveedor con Id: <strong>{Proveedor.SupplierID}</strong> - Nombre de compañía: <strong>{Proveedor.CompanyName}</strong>:</p>{resultado.Mensaje}";
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        BloquearEliminacion = true;
                }
            }
            return Page();
        }
    }
}
