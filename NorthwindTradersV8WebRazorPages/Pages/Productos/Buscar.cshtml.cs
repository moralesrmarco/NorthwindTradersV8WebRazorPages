using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class BuscarModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        private readonly CategoriaService categoriaService;
        private readonly ProveedorService proveedorService;
        [BindProperty(SupportsGet = true)]
        public ProductosBuscarDto Filtro { get; set; } = new ProductosBuscarDto();
        public DataTable Productos { get; set; } = new DataTable();
        public bool SeBusco { get; set; }
        public required List<SelectListItem> Categorias { get; set; }
        public required List<SelectListItem> Proveedores { get; set; }
        public BuscarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            categoriaService = new CategoriaService(connectionString);
            proveedorService = new ProveedorService(connectionString);
        }
        public void OnGet()
        {
            CargarCombos();
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
                Productos = productoBLL.BuscarProductos(Filtro);
            else
                Productos = new DataTable();
        }
        private void CargarCombos()
        {
            Categorias = categoriaService.ObtenerCategoriasCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text
            }).ToList();
            Proveedores = proveedorService.ObtenerProveedoresCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }
    }
}
