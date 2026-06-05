using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class FotoModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;

        public FotoModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            empleadoBLL = new EmpleadoBLL(connectionString);
        }
        public IActionResult OnGet(int id)
        {
            var empleadoFoto = empleadoBLL.ObtenerEmpleadoFotoPorId(id);
            if (empleadoFoto == null)
            {
                // Imagen por defecto si no hay foto
                return File("~/images/FotoPerfil.Png", "image/png");
            }

            return File(empleadoFoto, "image/jpeg");
        }
    }
}
