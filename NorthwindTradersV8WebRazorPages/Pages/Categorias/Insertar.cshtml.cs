using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class InsertarModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        [BindProperty]
        public Categoria? Categoria { get; set; } = new Categoria();
        [BindProperty]
        public IFormFile? PictureFile { get; set; }
        [BindProperty]
        public string? PictureTemporalBase64 { get; set; }
        [BindProperty]
        public string? PictureMime { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public bool BloquearEdicion { get; set; }
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            byte[]? pictureBytes = null;
            // Guardar la imagen temporal primero
            if (PictureFile != null && PictureFile.Length > 0)
            {
                using var ms = new MemoryStream();
                PictureFile.CopyTo(ms);

                pictureBytes = ms.ToArray();

                PictureTemporalBase64 = Convert.ToBase64String(ms.ToArray());
                PictureMime = PictureFile.ContentType;
            }
            // Validciones en el servidor
            if (string.IsNullOrWhiteSpace(Categoria?.CategoryName))
                ModelState.AddModelError("Categoria.CategoryName", "El nombre de la categoría es obligatorio");

            //if (Categoria?.Description == null || Categoria.Description == 0)
            //    ModelState.AddModelError("Categoria.Description", "Seleccione una descripción para la categoría");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (Categoria != null)
                {
                    if (pictureBytes != null)
                    {
                        Categoria.Picture = pictureBytes;
                    }
                    else if (!string.IsNullOrEmpty(PictureTemporalBase64))
                    {
                        // Reutilizamos la foto temporal
                        Categoria.Picture = Convert.FromBase64String(PictureTemporalBase64);
                    }
                    else
                    {
                        // Foto por defecto
                        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Categorias.png");
                        if (System.IO.File.Exists(defaultImagePath))
                        {
                            Categoria.Picture = System.IO.File.ReadAllBytes(defaultImagePath);
                        }
                    }
                    var resultado = categoriaBLL.Insertar(Categoria);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return LocalRedirect(ReturnUrl);

                        return RedirectToPage("Index");
                    }
                    TempData["Error"] = $"<p>La categoría <strong>{Categoria.CategoryName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al insertar la categoría <strong>{Categoria?.CategoryName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            return Page();
        }
    }
}
