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
            bool isValid = true;
            // Validciones manuales
            if (Producto.Categoria == null || Producto.Categoria.CategoryID == 0)
            {
                ModelState.AddModelError("Producto.Categoria.CategoryID", "Seleccione una categoría");
                isValid = false;
            }

            if (Producto.Proveedor == null || Producto.Proveedor.SupplierID == 0)
            {
                ModelState.AddModelError("Producto.Proveedor.SupplierID", "Seleccione un proveedor");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Producto.ProductName))
            {
                ModelState.AddModelError("Producto.ProductName", "Ingrese producto");
                isValid = false;
            }

            if (Producto.UnitPrice == null || Producto.UnitPrice == 0)
            {
                ModelState.AddModelError("Producto.UnitPrice", "Ingrese precio");
                isValid = false;
            }
            if (!isValid)
            {
                CargarCombos();
                return Page();
            }
            if (Producto != null)
            {
                var resultado = productoBLL.Insertar(Producto);
                if (resultado.Exito)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["Error"] = $"<p>El producto <strong>{Producto.ProductName}</strong>:</p>{resultado.Mensaje}";
                }
            }
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
