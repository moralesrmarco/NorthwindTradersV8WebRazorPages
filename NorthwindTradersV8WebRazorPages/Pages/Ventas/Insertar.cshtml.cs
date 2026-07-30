using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.ViewModels;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class InsertarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
        private readonly ClienteService clienteService;
        private readonly EmpleadoService empleadoService;
        private readonly TransportistaService transportistaService;
        private readonly CategoriaService categoriasService;
        private readonly ProductoService productoService;
        private readonly VentaService ventaService;
        [BindProperty]
        public VentaInsertarViewModel VentaVM { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Vendedores { get; set; }
        public List<SelectListItem> Transportistas { get; set; }
        public List<SelectListItem> Categorias { get; set; }
        public List<SelectListItem> Productos { get; set; }
        [BindProperty]
        public VentaDetalleViewModel Detalle { get; set; } = new();
        public List<VentaDetalleViewModel> Detalles { get; set; } = new();
        private const string SessionDetalleVenta = "DetalleVenta";
        [BindProperty]
        public VentaTotalesViewModel Totales { get; set; } = new();
        public string DetallesJson => JsonSerializer.Serialize(
            Detalles,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        public string TotalesJson => JsonSerializer.Serialize(
            Totales,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString);
            clienteService = new ClienteService(connectionString);
            empleadoService = new EmpleadoService(connectionString);
            transportistaService = new TransportistaService(connectionString);
            categoriasService = new CategoriaService(connectionString);
            productoService = new ProductoService(connectionString);
            ventaService = new VentaService(connectionString);
            Productos = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "»--- Seleccione una categoría ---«"
                }
            };
        }
        public void OnGet(bool nueva = true)
        {
            if (nueva)
            {
                HttpContext.Session.Remove(SessionDetalleVenta);
            }
            VentaVM.OrderDate = DateTime.Today;
            CargarCombos();
            Detalles = ObtenerDetalle();
            Totales = CalcularTotalesVenta(Detalles);
        }
        public IActionResult OnGetNueva(string? returnUrl)
        {
            HttpContext.Session.Remove(SessionDetalleVenta);
            return RedirectToPage(new
            {
                returnUrl
            });
        }
        [IgnoreAntiforgeryToken]
        public IActionResult OnPostAgregarDetalle(
            [FromBody] VentaDetalleViewModel detalle)
        {
            if (detalle.CategoriaID <= 0)
            {
                return BadRequest("Debe seleccionar una categoría.");
            }

            if (detalle.ProductID <= 0)
            {
                return BadRequest("Debe seleccionar un producto.");
            }

            if (detalle.Quantity <= 0)
            {
                return BadRequest("La cantidad debe ser mayor que cero.");
            }
            if (detalle.Quantity > detalle.UnitsInStock)
            {
                return BadRequest("La cantidad no puede ser mayor que las unidades en inventario.");
            }
            var lista = ObtenerDetalle();
            lista.Add(detalle);
            GuardarDetalle(lista);
            var totales = CalcularTotalesVenta(lista);
            return new JsonResult(new
            {
                count = lista.Count,
                lista = lista,
                totales = totales
            });
        }
        private void CargarCombos()
        {
            Clientes = clienteService.ObtenerClientesCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text,
            }).ToList();
            Vendedores = empleadoService.ObtenerEmpleadoEmpleadosCbo().Select(e => new SelectListItem
            {
                Value = e.Value,
                Text = e.Text
            }).ToList();
            Vendedores.RemoveAll(v => v.Text == "N/A");
            Transportistas = transportistaService.ObtenerTransportistasCbo().Select(t => new SelectListItem
            {
                Value= t.Value,
                Text= t.Text,
            }).ToList();
            Categorias = categoriasService.ObtenerCategoriasCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text
            }).ToList();
        }
        public JsonResult OnGetProductosPorCategoria(int categoriaId)
        {
            var productos = productoService.ObtenerProductosPorCategoriaCbo(categoriaId);
            return new JsonResult(productos);
        }
        public JsonResult OnGetUltimaInformacionEnvio(string customerId)
        {
            var informacion = ventaService.ObtenerUltimaInformacionDeEnvio(customerId);
            return new JsonResult(informacion);
        }
        public JsonResult OnGetProductoCostoEInventario(int productId)
        {
            var producto = productoService.ObtenerProductoCostoEInventario(productId);
            return new JsonResult(producto);
        }
        [IgnoreAntiforgeryToken]
        public JsonResult OnPostCalcularDetalle([FromBody] VentaDetalleViewModel detalle)
        {
            return new JsonResult(detalle);
        }
        private VentaTotalesViewModel CalcularTotalesVenta(List<VentaDetalleViewModel> lista)
        {
            return new VentaTotalesViewModel
            {
                NumeroProductos = lista.Count,
                TotalUnidades = lista.Sum(x => x.Quantity),
                TotalImporteConIVA =
                    lista.Sum(x => x.SubtotalDelImporteConIVAIncluido),
                TotalDescuento =
                    lista.Sum(x => x.SubtotalDelAhorroTotalDespuesDescuento),
                TotalImporteConDescuento =
                    lista.Sum(x => x.SubtotalDelImporteConIVAConDescuento),
                TotalImporteSinIVA =
                    lista.Sum(x => x.SubtotalDelImporteSinIVAConDescuento),
                TotalIVA =
                    lista.Sum(x => x.SubtotalIVADespuesDelDescuento),
                Total =
                    lista.Sum(x => x.Subtotal)
            };
        }
        public IActionResult OnPostGenerarVenta()
        {
            CargarCombos();
            if (!VentaVM.EmployeeID.HasValue || VentaVM.EmployeeID <= 0)
            {
                ModelState.AddModelError(
                    "VentaVM.EmployeeID",
                    "Debe seleccionar un vendedor.");
            }
            var lista = ObtenerDetalle();
            if (lista.Count == 0)
            {
                TempData["Error"] = "Debe agregar al menos un producto a la venta.";
            }
            if (!ModelState.IsValid || lista.Count == 0)
            {
                Detalles = lista;
                Totales = CalcularTotalesVenta(lista);
                return Page();
            }

            // Guardar la venta
            // Guardar encabezado
            // int idVenta = ventaBLL.Insertar(...);

            // Guardar detalle
            // foreach(var d in lista)
            //     ventaDetalleBLL.Insertar(idVenta,d);

            HttpContext.Session.Remove(SessionDetalleVenta);

            TempData["Mensaje"] = "La venta se registró correctamente.";

            return RedirectToPage("Insertar");
        }
        public IActionResult OnPostNuevaVenta()
        {
            HttpContext.Session.Remove(SessionDetalleVenta);
            return RedirectToPage();
        }
        //public IActionResult OnPostNotaRemision()
        //{
        //    if (_detalleTemporal.Count == 0)
        //    {
        //        TempData["Error"] = "Debe agregar productos.";
        //        CargarCombos();
        //        return Page();
        //    }

        //    //Guardar

        //    //Generar PDF

        //    return File(pdfBytes,
        //                "application/pdf",
        //                "NotaRemision.pdf");
        //}
        private List<VentaDetalleViewModel> ObtenerDetalle()
        {
            var json = HttpContext.Session.GetString(SessionDetalleVenta);

            if (string.IsNullOrEmpty(json))
                return new List<VentaDetalleViewModel>();

            return JsonSerializer.Deserialize<List<VentaDetalleViewModel>>(json)!;
        }
        private void GuardarDetalle(List<VentaDetalleViewModel> lista)
        {
            var json = JsonSerializer.Serialize(lista);

            HttpContext.Session.SetString(SessionDetalleVenta, json);
        }
    }
}
