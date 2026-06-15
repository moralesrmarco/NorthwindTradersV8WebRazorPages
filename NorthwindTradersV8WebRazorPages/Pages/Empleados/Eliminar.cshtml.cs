using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class EliminarModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        [BindProperty]
        public Empleado? Empleado { get; set; } = new Empleado();
        public bool BloquearEliminacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public EliminarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            empleadoBLL = new EmpleadoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var empleado = empleadoBLL.ObtenerEmpleadoPorId(id);
            if (empleado == null)
            {
                TempData["Error"] = "<p>Empleado no encontrado</p>" + StringsCommons.Nefep;
                BloquearEliminacion = true;
            }
            else
                Empleado = empleado;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Empleado != null)
            {
                var resultado = empleadoBLL.Eliminar(Empleado);
                if (resultado.Exito)
                    return RedirectToPage("Index");
                else
                {
                    TempData["Error"] = $"<p>El empleado con Id: <strong>{Empleado.EmployeeID}</strong> - Nombre de empleado: <strong>{Empleado.NameByFirstName}</strong>:</p>{resultado.Mensaje}";
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        BloquearEliminacion = true;
                }
            }
            return Page();
        }
    }
}
