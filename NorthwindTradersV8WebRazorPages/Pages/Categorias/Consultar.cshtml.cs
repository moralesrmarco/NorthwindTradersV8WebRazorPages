using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class ConsultarModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        [BindProperty]
        public Categoria? Categoria { get; set; } = new Categoria();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public ConsultarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var categoria = categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
                TempData["Error"] = "<p>Categoria no encontrada</p>" + StringsCommons.Nefep;
            else
                Categoria = categoria;
            return Page();
        }
    }
}
