using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos.Consultas
{
    public class ListadoProductosModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        private readonly CategoriaService categoriaService;
        private readonly ProveedorService proveedorService;
        [BindProperty]
        public string PestanaActiva { get; set; } = "#buscar";
        [BindProperty]
        public ProductosBuscarDto Filtro { get; set; }
        public List<ProductoDto> Productos { get; set; } = new();
        public required List<SelectListItem> Categorias { get; set; }
        public required List<SelectListItem> Proveedores { get; set; }
        private readonly List<KeyValuePair<string, string>> _itemsOrdenadoPor =
            new()
            {
                new("ProductID", "ID Producto"),
                new("ProductName", "Producto"),
                new("QuantityPerUnit", "Cantidad por unidad"),
                new("UnitPrice", "Precio"),
                new("UnitsInStock", "Unidades en inventario"),
                new("UnitsOnOrder", "Unidades en pedido"),
                new("ReorderLevel", "Nivel de reorden"),
                new("Discontinued", "Descontinuado"),
                new("CategoryName", "Categoría"),
                new("CompanyName", "Proveedor")
            };
        private readonly List<KeyValuePair<string, string>> _itemsAscDesc =
            new()
            {
            new("ASC", "Ascendente"),
            new("DESC", "Descendente")
            };
        public IEnumerable<SelectListItem> CamposOrden { get; private set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Direcciones { get; private set; } = Enumerable.Empty<SelectListItem>(); 
        public ListadoProductosModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            categoriaService = new CategoriaService(connectionString);
            proveedorService = new ProveedorService(connectionString);
            Filtro = new ProductosBuscarDto();
        }
        public void OnGet()
        {
            Filtro = new ProductosBuscarDto
            {
                IdIni = null,
                IdFin = null,
                Producto = string.Empty,
                Categoria = 0,
                Proveedor = 0,
                OrdenadoPor = "ProductID",
                AscDesc = "DESC"
            };
            CargarCombos();
            Productos = ObtenerDatos();
        }
        public void OnPost()
        {
            CargarCombos();
            Productos = ObtenerDatos();
        }
        private void CargarCombos()
        {
            CamposOrden = _itemsOrdenadoPor.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value
            });
            Direcciones = _itemsAscDesc.Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value
            });
            Categorias = categoriaService.ObtenerCategoriasCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text
            }).ToList();
            Proveedores = proveedorService.ObtenerProveedoresCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }
        private List<ProductoDto> ObtenerDatos()
        {
            return productoBLL.ObtenerProductosRpt(Filtro);
        }
    }
}
