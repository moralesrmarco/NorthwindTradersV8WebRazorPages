using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    // La imagen se muestra de dos formas:
    //
    // 1. GET inicial:
    //    Se obtiene mediante el endpoint /Categorias/Picture.
    //
    // 2. Después de seleccionar una nueva imagen o cuando el POST
    //    regresa por errores de validación:
    //    Se utiliza PictureTemporalBase64 para conservar la vista previa.
    // esta mal hay que considerar corregirlo y usar solo el mecanismo del endpoint, 
    public class EditarModel : PageModel
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
        public EditarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            Categoria = categoriaBLL.ObtenerCategoriaPorId(id);
            if (Categoria == null)
            {
                TempData["Error"] = "<p>Categoría no encontrada</p>" + Common.StringsCommons.Nefep;
                BloquearEdicion = true;
            }
            else if (Categoria?.Picture != null)
            {
                PictureTemporalBase64 = Convert.ToBase64String(Categoria.Picture);
                PictureMime = "image/jpeg"; // Ajusta esto según el tipo de imagen real
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Categoria?.CategoryID <= 9)
            {
                PictureFile = null;
                PictureTemporalBase64 = null;
            }
            byte[]? pictureBytes = null;
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

            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (Categoria != null)
                {
                    if (Categoria.CategoryID <= 9)
                    {
                        // Recuperar la foto original para que nunca se modifique
                        var categoriaOriginal = categoriaBLL.ObtenerCategoriaPorId(Categoria.CategoryID);

                        if (categoriaOriginal != null)
                        {
                            Categoria.Picture = categoriaOriginal.Picture;
                        }
                    }
                    else
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
                            // Recuperar la foto original de la BD
                            var categoriaOriginal =
                                categoriaBLL.ObtenerCategoriaPorId(Categoria.CategoryID);

                            if (categoriaOriginal != null)
                            {
                                Categoria.Picture = categoriaOriginal.Picture;
                            }
                        }
                    }
                    var resultado = categoriaBLL.Actualizar(Categoria);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return LocalRedirect(ReturnUrl);

                        return RedirectToPage("Index");
                    }
                    else
                    {
                        TempData["Error"] = $"<p>La categoría <strong>{Categoria.CategoryName}</strong>:</p>{resultado.Mensaje}";
                        if (resultado.Codigo < 0)
                            BloquearEdicion = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al actualizar la categoría <strong>{Categoria?.CategoryName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            return Page();
        }
    }
}
