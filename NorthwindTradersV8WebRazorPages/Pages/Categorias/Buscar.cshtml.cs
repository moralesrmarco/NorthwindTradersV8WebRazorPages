using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class BuscarModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        // Propiedades que se enlazan con los inputs del formulario
        [BindProperty(SupportsGet = true)]
        public CategoriasBuscarDto Filtro { get; set; } = new CategoriasBuscarDto();
        public DataTable Categorias { get; set; } = new DataTable();
        public bool SeBusco { get; set; }
        public BuscarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet()
        {
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
                Categorias = categoriaBLL.BuscarCategorias(Filtro);
            else
                Categorias = new DataTable();
        }
    }
}
