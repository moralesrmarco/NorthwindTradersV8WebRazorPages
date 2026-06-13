using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class ConsultarModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        [BindProperty]
        public Cliente? Cliente { get; set; } = new Cliente();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            clienteBLL = new ClienteBLL(connectionString);
        }
        public IActionResult OnGet(string id)
        {
            var cliente = clienteBLL.ObtenerClientePorId(id);
            if (cliente == null)
                TempData["Error"] = "<p>Cliente no encontrado</p>" + StringsCommons.Nefep;
            else
                Cliente = cliente;
            return Page();
        }
    }
}
