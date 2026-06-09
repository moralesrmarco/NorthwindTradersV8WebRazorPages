using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class BuscarModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        private readonly EmpleadoService empleadoService;
        // Propiedades que se enlazan con los inputs del formulario
        [BindProperty(SupportsGet = true)]
        public EmpleadosBuscarDto Filtro { get; set; } = new EmpleadosBuscarDto();
        public DataTable Empleados { get; set; } = new DataTable();
        public bool SeBusco { get; set; }
        public required List<SelectListItem> Paises { get; set; }

        public BuscarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            empleadoBLL = new EmpleadoBLL(connectionString);
            empleadoService = new EmpleadoService(connectionString);
        }
        public void OnGet()
        {
            CargarCombo();
            if (Filtro.IdIni.HasValue && Filtro.IdIni <= 0)
            {
                ModelState.AddModelError("Filtro.IdIni",
                    "El Id inicial debe ser mayor que cero");
            }

            if (Filtro.IdFin.HasValue && Filtro.IdFin <= 0)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor que cero");
            }

            if (Filtro.IdIni.HasValue &&
                Filtro.IdFin.HasValue &&
                Filtro.IdIni > Filtro.IdFin)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor o igual al Id inicial");
            }
            SeBusco = Request.Query.Count > 0;
            if (SeBusco)
                Empleados = empleadoBLL.BuscarEmpleados(Filtro);
            else
                Empleados = new DataTable();
        }
        private void CargarCombo()
        {
            Paises = empleadoService.ObtenerEmpleadosPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }
    }
}
