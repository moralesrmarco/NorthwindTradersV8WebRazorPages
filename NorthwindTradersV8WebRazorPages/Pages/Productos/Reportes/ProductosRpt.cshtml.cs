using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos.Reportes
{
    public class ProductosRptModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        private readonly CategoriaService categoriaService;
        private readonly ProveedorService proveedorService;
        public ProductosRptModel(IConfiguration configuration)
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
        [BindProperty]
        public string PestanaActiva { get; set; } = "#todos";
        [BindProperty]
        public ProductosBuscarDto Filtro { get; set; }
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
        private String titulo = "Reporte de todos los productos";
        private string subtitulo = "";
        public void OnGet()
        {
            CargarCombos();
        }
        //public IActionResult OnPostTodos()
        //{
        //    CargarCombos();
        //    return Page();
        //}

        //public IActionResult OnPostBuscar()
        //{
        //    CargarCombos();
        //    return Page();
        //}
        private List<ProductoDto> ObtenerDatosReporte()
        {
            if (PestanaActiva == "#todos")
            {
                Filtro.IdIni = 0;
                Filtro.IdFin = 0;
                Filtro.Producto = string.Empty;
                Filtro.Categoria = 0;
                Filtro.Proveedor = 0;
            }
            return productoBLL.ObtenerProductosRpt(Filtro);
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
        public IActionResult OnPostVerPdf()
        {
            CargarCombos();
            var reporte = CrearReporte();
            return File(reporte.Render("PDF"), "application/pdf");
        }

        public IActionResult OnPostExcel()
        {
            var reporte = CrearReporte();
            return File(
                reporte.Render("EXCELOPENXML"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Productos.xlsx");
        }

        public IActionResult OnPostWord()
        {
            var reporte = CrearReporte();
            return File(
                reporte.Render("WORDOPENXML"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Productos.docx");
        }
        private LocalReport CrearReporte()
        {
            ConstruirTituloSubtitulo();
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Pages",
                "Productos",
                "Reportes",
                "RptProductos.rdlc");
            reporte.DataSources.Clear();
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", titulo),
                new ReportParameter("subtitulo", subtitulo)
            }); 
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", ObtenerDatosReporte()));
            return reporte;
        }
        private void ConstruirTituloSubtitulo()
        {
            titulo = "» Reporte de todos los productos «";
            subtitulo = "";
            if (PestanaActiva == "#todos")
            {
                subtitulo = $"Ordenado por: [ {ObtenerTextoOrden()} ] [ {ObtenerTextoDireccion()} ]";
                return;
            }
            titulo = "» Reporte filtrado de productos «";
            subtitulo = "Filtrado por: ";
            if (Filtro.IdIni != 0 && Filtro.IdFin != 0)
                subtitulo += $" [ Id: {Filtro.IdIni} al {Filtro.IdFin} ] ";
            if (!string.IsNullOrWhiteSpace(Filtro.Producto))
                subtitulo += $" [ Producto: {Filtro.Producto} ] ";
            if (Filtro.Categoria > 0)
            {
                var categoria = Categorias.FirstOrDefault(c => c.Value == Filtro.Categoria.ToString());
                if (categoria != null)
                    subtitulo += $" [ Categoría: {categoria.Text} ] ";
            }
            if (Filtro.Proveedor > 0)
            {
                var proveedor = Proveedores.FirstOrDefault(p => p.Value == Filtro.Proveedor.ToString());
                if (proveedor != null)
                    subtitulo += $" [ Proveedor: {proveedor.Text} ] ";
            }
            if (subtitulo == "Filtrado por: ")
            {
                titulo = "» Reporte de todos los productos «";
                subtitulo = "";
            }
            if (!string.IsNullOrEmpty(subtitulo))
                subtitulo += $" Ordenado por: [ {ObtenerTextoOrden()} ] [ {ObtenerTextoDireccion()} ]";
        }
        private string ObtenerTextoOrden()
        {
            return _itemsOrdenadoPor
                .FirstOrDefault(x => x.Key == Filtro.OrdenadoPor)
                .Value ?? "";
        }
        private string ObtenerTextoDireccion()
        {
            return _itemsAscDesc
                .FirstOrDefault(x => x.Key == Filtro.AscDesc)
                .Value ?? "";
        }
    }
}
