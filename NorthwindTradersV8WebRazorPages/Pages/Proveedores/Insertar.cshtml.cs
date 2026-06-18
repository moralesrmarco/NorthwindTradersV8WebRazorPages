using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class InsertarModel : PageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        private readonly ProveedorService proveedorService;
        [BindProperty]
        public Proveedor? Proveedor { get; set; } = new Proveedor();
        public required List<SelectListItem> Paises { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public bool BloquearEdicion { get; set; }

        public InsertarModel(IConfiguration configuration)
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
        }
        public IActionResult OnPost()
        {
            // Validciones en el servidor
            if (string.IsNullOrEmpty(Proveedor?.Country)
                || Proveedor.Country == "0")
                ModelState.AddModelError("Proveeedor.Country", "Seleccione o escriba un país");
            if (!ModelState.IsValid)
            {
                CargarCombo();
                return Page();
            }
            try
            {
                if (Proveedor != null)
                {
                    var resultado = proveedorBLL.Insertar(Proveedor);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return LocalRedirect(ReturnUrl);

                        return RedirectToPage("Index");
                    }
                    TempData["Error"] = $"<p>El proveedor <strong>{Proveedor.CompanyName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al insertar el proveedor <strong>{Proveedor?.CompanyName}</strong>.</p><p>Detalles: {ex.Message}</p>";
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
                && !Paises.Any(p => p.Value == Proveedor.Country))
            {
                Paises.Add(new SelectListItem { Value = Proveedor.Country, Text = Proveedor.Country });
            }
        }
    }
}
