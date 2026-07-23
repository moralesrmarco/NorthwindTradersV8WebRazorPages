using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class InsertarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        private readonly ClienteService clienteService;
        private readonly EmpleadoService empleadoService;
        private readonly TransportistaService transportistaService;

        [BindProperty]
        public VentaDto? Venta { get; set; } = new VentaDto();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public required List<SelectListItem> Clientes { get; set; }
        public required List<SelectListItem> Vendedores { get; set; }
        public required List<SelectListItem> Transportistas { get; set; }
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            clienteService = new ClienteService(connectionString);
            empleadoService = new EmpleadoService(connectionString);
            transportistaService = new TransportistaService(connectionString);
        }
        public void OnGet()
        {
            Venta.OrderDate = DateTime.Today;
            Venta.RequiredDate = DateTime.Today;
            Venta.ShippedDate = null;
            CargarCombos();
        }
        private void CargarCombos()
        {
            Clientes = clienteService.ObtenerClientesCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text,
            }).ToList();
            Vendedores = empleadoService.ObtenerEmpleadoEmpleadosCbo().Select(e => new SelectListItem
            {
                Value = e.Value,
                Text = e.Text
            }).ToList();
            Vendedores.RemoveAll(v => v.Text == "N/A");
            Transportistas = transportistaService.ObtenerTransportistasCbo().Select(t => new SelectListItem
            {
                Value= t.Value,
                Text= t.Text,
            }).ToList();
        }
    }
}
