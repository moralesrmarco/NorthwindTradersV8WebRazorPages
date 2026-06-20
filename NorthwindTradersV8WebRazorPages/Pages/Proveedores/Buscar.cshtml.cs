using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class BuscarModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        private readonly ProveedorService proveedorService;
        [BindProperty(SupportsGet = true)]
        public ProveedoresBuscarDto Filtro { get; set; } = new ProveedoresBuscarDto();
        public DataTable Proveedores { get; set; } = new DataTable();
        public bool SeBusco { get; set; }
        public required List<SelectListItem> Paises { get; set; }
        public BuscarModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found.");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            proveedorBLL = new ProveedorBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            proveedorService = new ProveedorService(connectionString);
        }

        public void OnGet()
        {
            CargarCombo();
            SeBusco = Request.Query.Count > 0;
            if (SeBusco)
                Proveedores = proveedorBLL.BuscarProveedores(Filtro);
            else
                Proveedores = new DataTable();
        }
        private void CargarCombo()
        {
            Paises = proveedorService.ObtenerProveedoresPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }
    }
}
