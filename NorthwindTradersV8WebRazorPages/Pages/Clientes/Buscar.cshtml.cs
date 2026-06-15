using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class BuscarModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        private readonly ClienteService clienteService;
        [BindProperty(SupportsGet = true)]
        public ClientesBuscarDto Filtro { get; set; } = new ClientesBuscarDto();
        public DataTable Clientes { get; set; } = new DataTable();
        public bool SeBusco { get; set; }
        public required List<SelectListItem> Paises { get; set; }
        public BuscarModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found.");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            clienteService = new ClienteService(connectionString);
        }

        public void OnGet()
        {
            CargarCombo();
            SeBusco = Request.Query.Count > 0;
            if (SeBusco)
                Clientes = clienteBLL.BuscarClientes(Filtro);
            else
                Clientes = new DataTable();
        }
        private void CargarCombo()
        {
            Paises = clienteService.ObtenerClientesPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }

    }
}
