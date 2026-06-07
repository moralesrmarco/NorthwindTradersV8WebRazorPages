using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class EditarModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        private readonly EmpleadoService empleadoService;
        [BindProperty]
        public Empleado? Empleado { get; set; } = new Empleado();
        public required List<SelectListItem> ReportaA { get; set; }
        public required List<SelectListItem> Paises { get; set; }
        [BindProperty]
        public IFormFile? Foto { get; set; }

        //el navegador borra el archivo seleccionado si hay un error de validación y se regresa con return Page(). Por seguridad, nunca se conserva el archivo en el<input type="file">. Por eso tu código termina usando la imagen por defecto y el usuario no se da cuenta.
        [BindProperty]
        public string? FotoTemporalBase64 { get; set; }
        [BindProperty]
        public string? FotoMime { get; set; }
        public EditarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            empleadoBLL = new EmpleadoBLL(connectionString);
            empleadoService = new EmpleadoService(connectionString);
        }
        public IActionResult OnGet(int id)
        {
            Empleado = empleadoBLL.ObtenerEmpleadoPorId(id);
            if (Empleado == null)
            {
                TempData["Error"] = "<p>Empleado no encontrado</p>" + Common.StringsCommons.Nefep;
                return RedirectToPage("Index");
            }
            CargarCombos();
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Empleado?.EmployeeID <= 9)
            {
                Foto = null;
                FotoTemporalBase64 = null;
            }
            byte[]? fotoBytes = null;
            // Guardar la imagen temporal primero
            if (Foto != null && Foto.Length > 0)
            {
                using var ms = new MemoryStream();
                Foto.CopyTo(ms);
                fotoBytes = ms.ToArray();
                FotoTemporalBase64 = Convert.ToBase64String(ms.ToArray());
                FotoMime = Foto.ContentType;
            }
            // Validaciones en el servidor
            if (string.IsNullOrEmpty(Empleado?.Country)
                || Empleado.Country == "0")
                ModelState.AddModelError("Empleado.Country", "Seleccione o escriba un país termine con un tab cuando inserte un nuevo país");
            if (Empleado?.ReportsTo == null || Empleado.ReportsTo == 0)
                ModelState.AddModelError("Empleado.ReportsTo", "Seleccione a quién reporta el empleado");
            if (!ModelState.IsValid)
            {
                // Recargar listas si hay error de validación
                CargarCombos();
                return Page();
            }
            try
            {
                if (Empleado != null)
                {
                    if (Empleado.EmployeeID <= 9)
                    {
                        // Recuperar la foto original para que nunca se modifique
                        var empleadoOriginal = empleadoBLL.ObtenerEmpleadoPorId(Empleado.EmployeeID);

                        if (empleadoOriginal != null)
                        {
                            Empleado.Photo = empleadoOriginal.Photo;
                        }
                    }
                    else
                    {
                        if (fotoBytes != null)
                        {
                            Empleado.Photo = fotoBytes;
                        }
                        else if (!string.IsNullOrEmpty(FotoTemporalBase64))
                        {
                            Empleado.Photo = Convert.FromBase64String(FotoTemporalBase64);
                        }
                        else
                        {
                            var defaultImagePath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "images",
                                "FotoPerfil.png");

                            if (System.IO.File.Exists(defaultImagePath))
                            {
                                Empleado.Photo = System.IO.File.ReadAllBytes(defaultImagePath);
                            }
                        }
                    }
                    var resultado = empleadoBLL.Actualizar(Empleado);
                    if (resultado.Exito)
                        return RedirectToPage("Index");
                    TempData["Error"] = $"<p>El empleado <strong>{Empleado.FirstName} {Empleado.LastName}</strong>:</p>{resultado.Mensaje}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al actualizar el empleado <strong>{Empleado?.FirstName} {Empleado?.LastName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            CargarCombos();
            return Page();
        }
        private void CargarCombos()
        {
            Paises = empleadoService.ObtenerEmpleadosPaisesCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
            // 👇 Si el usuario escribió un país nuevo, lo agregamos para que se conserve
            if (!string.IsNullOrEmpty(Empleado?.Country)
                && !Paises.Any(p => p.Value == Empleado.Country))
            {
                Paises.Add(new SelectListItem { Value = Empleado.Country, Text = Empleado.Country });
            }
            ReportaA = empleadoService.ObtenerEmpleadoEmpleadosCbo().Select(e => new SelectListItem
            {
                Value = e.Value,
                Text = e.Text
            }).ToList();
            // 👇 Forzar N/A si no tiene jefe
            if (Empleado?.ReportsTo == null)
                Empleado?.ReportsTo = -1;
        }
        private string ObtenerMimeType(byte[] foto)
        {
            if (foto.Length >= 8)
            {
                // PNG
                if (foto[0] == 0x89 &&
                    foto[1] == 0x50 &&
                    foto[2] == 0x4E &&
                    foto[3] == 0x47)
                    return "image/png";

                // JPG/JPEG
                if (foto[0] == 0xFF &&
                    foto[1] == 0xD8)
                    return "image/jpeg";

                // GIF
                if (foto[0] == 0x47 &&
                    foto[1] == 0x49 &&
                    foto[2] == 0x46)
                    return "image/gif";

                // WEBP
                if (foto.Length >= 12 &&
                    foto[8] == 0x57 &&
                    foto[9] == 0x45 &&
                    foto[10] == 0x42 &&
                    foto[11] == 0x50)
                    return "image/webp";
            }

            return "application/octet-stream";
        }
    }
}
