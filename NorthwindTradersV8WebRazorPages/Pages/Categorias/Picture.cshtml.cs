using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class PictureModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;

        public PictureModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            categoriaBLL = new CategoriaBLL(connectionString);
        }
        public IActionResult OnGet(int id)
        {
            var categoriaPicture = categoriaBLL.ObtenerCategoriaPicturePorId(id);
            if (categoriaPicture == null)
            {
                // Imagen por defecto si no hay picture
                return File("~/images/Categorias.Png", "image/png");
            }
            return File(categoriaPicture, "image/jpeg");
        }
    }
}
