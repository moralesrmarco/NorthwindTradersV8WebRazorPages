using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class EditarModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        private readonly ClienteService clienteService;
        [BindProperty]
        public Cliente? Cliente { get; set; } = new Cliente();
        public required List<SelectListItem> Paises { get; set; }
        public bool BloquearEdicion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public string UrlCancelar =>
            string.IsNullOrEmpty(ReturnUrl)
                ? Url.Page("Index")!
                : ReturnUrl;
        public EditarModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found.");
            clienteBLL = new ClienteBLL(connectionString);
            clienteService = new ClienteService(connectionString);
        }
        public IActionResult OnGet(string id)
        {
            Cliente = clienteBLL.ObtenerClientePorId(id);
            if (Cliente == null)
            {
                TempData["Error"] = "<p>Cliente no encontrado</p>" + Common.StringsCommons.Nefep;
                BloquearEdicion = true;
            }
            CargarCombo();
            return Page();
        }
        public IActionResult OnPost()
        {
            // Validaciones en el servidor
            if (string.IsNullOrWhiteSpace(Cliente?.Country) || Cliente.Country == "0")
                ModelState.AddModelError("Cliente.Country", "Seleccione o escriba un país termine con un tab cuando inserte un nuevo país");
            if (!ModelState.IsValid)
            {
                CargarCombo();
                return Page();
            }
            try
            {
                if (Cliente != null)
                {
                    var resultado = clienteBLL.Actualizar(Cliente);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return LocalRedirect(ReturnUrl);
                        return RedirectToPage("Index");
                    }
                    TempData["Error"] = $"<p>El cliente <strong>{Cliente.CompanyName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al editar el cliente <strong>{Cliente?.CompanyName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            CargarCombo();
            return Page();
        }
        private void CargarCombo()
        {
            Paises = clienteService.ObtenerClientesPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
            // 👇 Si el usuario escribió un país nuevo, lo agregamos para que se conserve
            if (!string.IsNullOrEmpty(Cliente?.Country)
                && !Paises.Any(p => string.Equals(
                        p.Value, 
                        Cliente.Country,
                        StringComparison.OrdinalIgnoreCase
                    )))
            {
                Paises.Add(new SelectListItem { Value = Cliente.Country, Text = Cliente.Country });
            }
        }

    }
}
