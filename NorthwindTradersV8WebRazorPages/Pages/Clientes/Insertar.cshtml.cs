using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class InsertarModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        private readonly ClienteService clienteService;
        [BindProperty]
        public Cliente? Cliente { get; set; } = new Cliente();
        public required List<SelectListItem> Paises { get; set; }
        public InsertarModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found.");
            clienteBLL = new ClienteBLL(connectionString);
            clienteService = new ClienteService(connectionString);
        }
        public void OnGet()
        {
            CargarCombo();
        }
        public IActionResult OnPost()
        {
            // Validciones en el servidor
            if (string.IsNullOrEmpty(Cliente?.Country)
                || Cliente.Country == "0")
                ModelState.AddModelError("Cliente.Country", "Seleccione o escriba un país termine con un tab cuando inserte un nuevo país");
            if (!ModelState.IsValid)
            {
                CargarCombo();
                return Page();
            }
            // Validar ID duplicado
            if (Cliente != null && clienteBLL.ExisteCliente(Cliente.CustomerID))
            {
                ModelState.AddModelError(
                    "Cliente.CustomerID",
                    $"El ID del cliente {Cliente.CustomerID} ya existe. Proporcione un nuevo ID.");
                CargarCombo();
                return Page();
            }

            try
            {
                if (Cliente != null)
                {
                    var resultado = clienteBLL.Insertar(Cliente);
                    if (resultado.Exito)
                        return RedirectToPage("Index");
                    TempData["Error"] = $"<p>El cliente <strong>{Cliente.CompanyName}</strong>:</p>{resultado.Mensaje}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al insertar el cliente <strong>{Cliente?.CompanyName}</strong>.</p><p>Detalles: {ex.Message}</p>";
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
                && !Paises.Any(p => p.Value == Cliente.Country))
            {
                Paises.Add(new SelectListItem { Value = Cliente.Country, Text = Cliente.Country });
            }
        }
    }
}
