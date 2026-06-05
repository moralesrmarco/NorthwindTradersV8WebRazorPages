using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class ConsultarModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        [BindProperty]
        public Empleado? Empleado { get; set; } = new Empleado();
        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            empleadoBLL = new EmpleadoBLL(connectionString);
        }
        public IActionResult OnGet(int id)
        {
            var empleado = empleadoBLL.ObtenerEmpleadoPorId(id);
            if (empleado == null)
                TempData["Error"] = "<p>Empleado no encontrado</p>" + StringsCommons.Nefep;
            else
                Empleado = empleado;
            return Page();
        }
    }
}
