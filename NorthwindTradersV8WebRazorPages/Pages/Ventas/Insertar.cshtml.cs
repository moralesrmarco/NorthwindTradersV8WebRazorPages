using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.ViewModels;
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
        public bool VentaGuardada
        {
            get
            {
                return TempData["VentaGuardada"] != null;
            }
        }
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
            VentaVM.OrderTime = DateTime.Now.TimeOfDay; 
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
            // Validar producto duplicado
            if (lista.Any(x => x.ProductID == detalle.ProductID))
            {
                return BadRequest("No se puede agregar un mismo producto más de una vez a la venta.");
            }
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
            if (Detalle.CategoriaID.HasValue)
            {
                Productos = productoService
                    .ObtenerProductosPorCategoriaCbo(Detalle.CategoriaID.Value)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Value,
                        Text = p.Text
                    })
                    .ToList();
            }
            else
            {
                Productos = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "",
                        Text = "»--- Seleccione una categoría ---«"
                    }
                };
            }
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
            // Construir Fecha/Hora de la venta
            DateTime? fechaHoraVenta = null;
            if (VentaVM.OrderDate.HasValue && VentaVM.OrderTime.HasValue)
            {
                fechaHoraVenta = VentaVM.OrderDate.Value.Date + VentaVM.OrderTime.Value;
            }

            // Construir Fecha/Hora requerida
            DateTime? fechaHoraRequerido = null;
            if (VentaVM.RequiredDate.HasValue && VentaVM.RequiredTime.HasValue)
            {
                fechaHoraRequerido = VentaVM.RequiredDate.Value.Date + VentaVM.RequiredTime.Value;
            }

            // Construir Fecha/Hora de envío
            DateTime? fechaHoraEnvio = null;
            if (VentaVM.ShippedDate.HasValue && VentaVM.ShippedTime.HasValue)
            {
                fechaHoraEnvio = VentaVM.ShippedDate.Value.Date + VentaVM.ShippedTime.Value;
            }
            // 1. Validar Requerido vs Venta
            if (fechaHoraVenta.HasValue &&
                fechaHoraRequerido.HasValue &&
                fechaHoraRequerido.Value < fechaHoraVenta.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.RequiredTime",
                    "La fecha y hora requerida no pueden ser anteriores a la venta.");
            }
            // 2. Validar Envío vs Venta
            if (fechaHoraVenta.HasValue &&
                fechaHoraEnvio.HasValue &&
                fechaHoraEnvio.Value < fechaHoraVenta.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.ShippedTime",
                    "La fecha y hora de envío no pueden ser anteriores a la venta.");
            }
            if (fechaHoraEnvio.HasValue &&
                fechaHoraRequerido.HasValue &&
                fechaHoraEnvio.Value > fechaHoraRequerido.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.ShippedTime",
                    "La fecha y hora de envío no pueden ser posteriores a la fecha y hora requeridas.");
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
            // Simular tiempo de guardado de la venta
            TempData["VentaGuardada"] = true;
            TempData["Mensaje"] = "La venta se registró correctamente.";
            Detalles = lista;
            Totales = CalcularTotalesVenta(lista);
            return Page();
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
        [IgnoreAntiforgeryToken]
        public IActionResult OnPostEliminarDetalle([FromBody] int indice)
        {
            var lista = ObtenerDetalle();

            if (indice < 0 || indice >= lista.Count)
            {
                return BadRequest("Producto no encontrado.");
            }

            lista.RemoveAt(indice);

            GuardarDetalle(lista);

            var totales = CalcularTotalesVenta(lista);

            return new JsonResult(new
            {
                lista,
                totales
            });
        }
        [IgnoreAntiforgeryToken]
        public IActionResult OnPostEditarDetalle([FromBody] int indice)
        {
            var lista = ObtenerDetalle();

            if (indice < 0 || indice >= lista.Count)
            {
                return BadRequest("Producto no encontrado.");
            }

            var detalle = lista[indice];

            // Lo quitamos temporalmente
            lista.RemoveAt(indice);

            GuardarDetalle(lista);

            var totales = CalcularTotalesVenta(lista);

            return new JsonResult(new
            {
                detalle,
                lista,
                totales
            });
        }
    }
}
