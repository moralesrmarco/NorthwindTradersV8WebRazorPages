using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class EliminarModel : PageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        [BindProperty]
        public Categoria? Categoria { get; set; } = new Categoria();
        public bool BloquearEliminacion { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public EliminarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public IActionResult OnGet(int id)
        {
            var categoria = categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
            {
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
                BloquearEliminacion = true;
            }
            else
                Categoria = categoria;
            return Page();
        }
        public IActionResult OnPost()
        {
            if (Categoria != null)
            {
                var resultado = categoriaBLL.Eliminar(Categoria);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return LocalRedirect(ReturnUrl);
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>La categoría con Id: <strong>{Categoria.CategoryID}</strong> - Nombre de categoría: <strong>{Categoria.CategoryName}</strong>:</p>{resultado.Mensaje}";
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        BloquearEliminacion = true;
                }
            }
            return Page();
        }
    }
}
