using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.Common;

namespace NorthwindTradersV8WebRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.TienePermiso(Permisos.TableroAltaDireccion))
            {
                return RedirectToPage(
                    "/Tableros/AltaDireccion/TableroControlAltaDireccion");
            }

            if (User.TienePermiso(Permisos.TableroVendedores))
            {
                return RedirectToPage(
                    "/Tableros/Vendedores/TableroControlVendedores");
            }

            return Page();
        }
    }
}
