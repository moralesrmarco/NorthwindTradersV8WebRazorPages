using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class EditarModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        private readonly ProveedorService proveedorService;
        [BindProperty]
        public Proveedor? Proveedor { get; set; } = new Proveedor();
        public required List<SelectListItem> Paises { get; set; }
        public bool BloquearEdicion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public EditarModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found.");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            proveedorBLL = new ProveedorBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            proveedorService = new ProveedorService(connectionString);
        }
        public IActionResult OnGet(string id)
        {
            Proveedor = proveedorBLL.ObtenerProveedorPorId(id);
            if (Proveedor == null)
            {
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + Common.StringsCommons.Nefep;
                BloquearEdicion = true;
            }
            CargarCombo();
            return Page();
        }
        public IActionResult OnPost()
        {
            //// Validaciones en el servidor
            if (string.IsNullOrWhiteSpace(Proveedor?.Country))
                ModelState.AddModelError("Proveedor.Country", "Seleccione o escriba un país");
            if (!ModelState.IsValid)
            {
                CargarCombo();
                return Page();
            }
            try
            {
                if (Proveedor != null)
                {
                    var resultado = proveedorBLL.Actualizar(Proveedor);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return LocalRedirect(ReturnUrl);
                        return RedirectToPage("Index");
                    }
                    TempData["Error"] = $"<p>El proveedor <strong>{Proveedor.CompanyName}</strong>: </p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al editar el proveedor <strong>{Proveedor?.CompanyName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            CargarCombo();
            return Page();
        }
        private void CargarCombo()
        {
            Paises = proveedorService.ObtenerProveedoresPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
            // 👇 Si el usuario escribió un país nuevo, lo agregamos para que se conserve
            if (!string.IsNullOrEmpty(Proveedor?.Country)
                && !Paises.Any(p => string.Equals(
                        p.Value,
                        Proveedor.Country,
                        StringComparison.OrdinalIgnoreCase
                    )))
            {
                Paises.Add(new SelectListItem { Value = Proveedor.Country, Text = Proveedor.Country });
            }
        }
    }
}
