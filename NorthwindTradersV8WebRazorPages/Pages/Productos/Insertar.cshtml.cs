using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class InsertarModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        private readonly CategoriaService categoriaService;
        private readonly ProveedorService proveedorService;
        [BindProperty]
        public Producto Producto { get; set; } = new Producto();
        public required List<SelectListItem> Categorias { get; set; }
        public required List<SelectListItem> Proveedores { get; set; }
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            productoBLL = new ProductoBLL(connectionString);
            categoriaService = new CategoriaService(connectionString);
            proveedorService = new ProveedorService(connectionString);
        }
        public void OnGet()
        {
            CargarCombos();
        }
        public IActionResult OnPost()
        {
            // Validciones manuales
            if (Producto?.Categoria == null || Producto.Categoria.CategoryID == 0)
                ModelState.AddModelError("Producto.Categoria.CategoryID", "Seleccione una categoría");

            if (Producto?.Proveedor == null || Producto.Proveedor.SupplierID == 0)
                ModelState.AddModelError("Producto.Proveedor.SupplierID", "Seleccione un proveedor");

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return Page();
            }
            try
            {
                if (Producto != null)
                {
                    var resultado = productoBLL.Insertar(Producto);
                    if (resultado.Exito)
                        return RedirectToPage("Index");
                    TempData["Error"] = $"<p>El producto <strong>{Producto.ProductName}</strong>:</p>{resultado.Mensaje}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"<p>Error al insertar el producto <strong>{Producto?.ProductName}</strong>.</p><p>Detalles: {ex.Message}</p>";
            }
            CargarCombos();
            return Page();
        }

        private void CargarCombos()
        {
            Categorias = categoriaService.ObtenerCategoriasCbo()
                .Select(c => new SelectListItem { Value = c.Value, Text = c.Text })
                .ToList();

            Proveedores = proveedorService.ObtenerProveedoresCbo()
                .Select(p => new SelectListItem { Value = p.Value, Text = p.Text })
                .ToList();
        }
    }
}
