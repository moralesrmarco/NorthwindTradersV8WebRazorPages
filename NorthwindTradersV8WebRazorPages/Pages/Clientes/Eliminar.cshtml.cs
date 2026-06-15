using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class EliminarModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        [BindProperty]
        public Cliente? Cliente { get; set; } = new Cliente();
        public bool BloquearEliminacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public string UrlCancelar =>
            string.IsNullOrEmpty(ReturnUrl)
                ? Url.Page("Index")!
                : ReturnUrl;
        public EliminarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(string id)
        {
            var cliente = clienteBLL.ObtenerClientePorId(id);
            if (cliente == null)
            {
                TempData["Error"] = "<p>Cliente no encontrado</p>" + StringsCommons.Nefep;
                BloquearEliminacion = true;
            }
            else
                Cliente = cliente;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Cliente != null)
            {
                var resultado = clienteBLL.Eliminar(Cliente);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return LocalRedirect(ReturnUrl);
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>El cliente con Id: <strong>{Cliente.CustomerID}</strong> - Nombre de compañía: <strong>{Cliente.CompanyName}</strong>:</p>{resultado.Mensaje}";
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        BloquearEliminacion = true;
                }
            }
            return Page();
        }

    }
}
