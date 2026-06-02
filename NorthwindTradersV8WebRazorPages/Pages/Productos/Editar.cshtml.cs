using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class EditarModel : PageModel
    {
        private readonly ProductoBLL _productoBLL;
        
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string Nombre { get; set; }
        [BindProperty]
        public decimal Precio { get; set; }
        public EditarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection");
            _productoBLL = new ProductoBLL(connectionString);
        }
        public IActionResult OnPost()
        {
            _productoBLL.Actualizar(Id, Nombre, Precio);
            return RedirectToPage("Index");
        }
        public void OnGet(int id)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);
            if (producto != null)
            {
                Id = (int)producto["ProductID"];
                Nombre = producto["ProductName"].ToString();
                Precio = (decimal)producto["UnitPrice"];
            }
        }
    }
}
