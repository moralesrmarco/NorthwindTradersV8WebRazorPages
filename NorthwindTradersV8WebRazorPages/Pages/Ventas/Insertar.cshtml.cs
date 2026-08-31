using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using NorthwindTradersV8WebRazorPages.ViewModels;
using System.Text.Json;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class InsertarModel : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly ClienteService clienteService;
        private readonly EmpleadoService empleadoService;
        private readonly TransportistaService transportistaService;
        private readonly CategoriaService categoriasService;
        private readonly ProductoService productoService;
        private readonly VentaService ventaService;
        private readonly TasaImpuestoBLL tasaImpuestoBLL;
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
        [BindProperty]
        public int OrderID { get; set; }
        public bool VentaTieneDetalles { get; set; }
        public InsertarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            clienteService = new ClienteService(connectionString);
            empleadoService = new EmpleadoService(connectionString);
            transportistaService = new TransportistaService(connectionString);
            categoriasService = new CategoriaService(connectionString);
            productoService = new ProductoService(connectionString);
            ventaService = new VentaService(connectionString);
            tasaImpuestoBLL = new TasaImpuestoBLL(connectionString);
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
            var ahora = DateTime.Now;
            VentaVM.OrderDate = ahora.Date;
            VentaVM.OrderTime = ahora.TimeOfDay;
            VentaVM.TasaIVA = ObtenerTasaIVA(VentaVM.OrderDate, VentaVM.OrderTime);
            CargarCombos();
            Detalles = ObtenerDetalle();
            VentaTieneDetalles = Detalles.Count > 0;
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
                return BadRequest(
                    "La cantidad no puede ser mayor que las unidades en inventario.");
            }

            var lista = ObtenerDetalle();

            // Validar producto duplicado
            if (lista.Any(x => x.ProductID == detalle.ProductID))
            {
                return BadRequest(
                    "No se puede agregar un mismo producto más de una vez a la venta.");
            }

            // =============================================
            // CALCULAR EL DETALLE
            // =============================================

            var ventaDetalle = detalle.ToVentaDetalle();

            var detalleCalculado =
                VentaDetalleViewModel.FromVentaDetalle(ventaDetalle);

            // =============================================
            // OBTENER LA TASA DE IVA VIGENTE
            // =============================================

            var fechaHoraVenta = CombinarFechaHora(
                VentaVM.OrderDate,
                VentaVM.OrderTime);

            // =============================================
            // CONSERVAR DATOS DE LA UI QUE NO PERTENECEN
            // AL CÁLCULO DE VentaDetalle
            // =============================================

            detalleCalculado.CategoriaID = detalle.CategoriaID;
            detalleCalculado.UnitsInStock = detalle.UnitsInStock;

            // =============================================
            // GUARDAR EN SESIÓN
            // =============================================

            lista.Add(detalleCalculado);

            GuardarDetalle(lista);

            var totales = CalcularTotalesVenta(lista);

            return new JsonResult(new
            {
                count = lista.Count,
                lista = lista,
                totales = totales,
                ventaTieneDetalles = lista.Count > 0
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
        public JsonResult OnPostCalcularDetalle(
            [FromBody] VentaDetalleViewModel detalle)
        {
            var ventaDetalle = detalle.ToVentaDetalle();

            // Aquí se ejecuta el cálculo de VentaDetalle

            var resultado = VentaDetalleViewModel.FromVentaDetalle(ventaDetalle);

            return new JsonResult(resultado);
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
            var fechaHoraVenta = CombinarFechaHora(
                VentaVM.OrderDate,
                VentaVM.OrderTime);
            var tasaIVA = tasaImpuestoBLL.ObtenerTasaVigente(
                fechaHoraVenta!.Value);
            if (!tasaIVA.HasValue)
            {
                ModelState.AddModelError(
                    "",
                    "No existe una tasa de IVA vigente para la fecha de la venta.");
                Detalles = lista;
                Totales = CalcularTotalesVenta(lista);
                return Page();
            }
            VentaVM.TasaIVA = tasaIVA.Value;
            // Construir Fecha/Hora requerida
            var fechaHoraRequerido = CombinarFechaHora(
                VentaVM.RequiredDate,
                VentaVM.RequiredTime);
            // Construir Fecha/Hora de envío
            var fechaHoraEnvio = CombinarFechaHora(
                VentaVM.ShippedDate,
                VentaVM.ShippedTime);            // 1. Validar Requerido vs Venta
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
            var venta = ConstruirVenta(lista);
            int orderId;
            byte[] rowVersion;
            ResultadoOperacion resultado =
                ventaBLL.InsertarVentaCompleta(
                    venta,
                    out orderId,
                    out rowVersion);
            if (!resultado.Exito)
            {
                ModelState.AddModelError("", resultado.Mensaje);
                Detalles = lista;
                Totales = CalcularTotalesVenta(lista);
                return Page();
            }
            HttpContext.Session.Remove(SessionDetalleVenta);
            OrderID = orderId;
            // Elimina el valor antiguo (0)
            ModelState.Remove(nameof(OrderID));
            TempData["VentaGuardada"] = true;
            TempData["Mensaje"] = $"La venta N° {orderId} se registró correctamente.";
            Detalles = lista;
            Totales = CalcularTotalesVenta(lista);
            return Page();
        }
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
                totales,
                ventaTieneDetalles = lista.Count > 0
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
        private Venta ConstruirVenta(List<VentaDetalleViewModel> detalles)
        {
            var fechaHoraVenta = CombinarFechaHora(VentaVM.OrderDate, VentaVM.OrderTime);
            var fechaHoraRequerido = CombinarFechaHora(VentaVM.RequiredDate, VentaVM.RequiredTime);
            var fechaHoraEnvio = CombinarFechaHora(VentaVM.ShippedDate, VentaVM.ShippedTime);
            var venta = new Venta
            {
                Cliente = new Cliente
                {
                    CustomerID = VentaVM.CustomerID
                },
                Empleado = new Empleado
                {
                    EmployeeID = VentaVM.EmployeeID.Value
                },
                Transportista = new Transportista
                {
                    ShipperID = VentaVM.ShipVia ?? 0
                },
                OrderDate = fechaHoraVenta,
                RequiredDate = fechaHoraRequerido,
                ShippedDate = fechaHoraEnvio,
                ShipName = VentaVM.ShipName,
                ShipAddress = VentaVM.ShipAddress,
                ShipCity = VentaVM.ShipCity,
                ShipRegion = VentaVM.ShipRegion,
                ShipPostalCode = VentaVM.ShipPostalCode,
                ShipCountry = VentaVM.ShipCountry,
                Freight = VentaVM.Freight,
                VentaDetalles = new List<VentaDetalle>()
            };
            foreach (var item in detalles)
            {
                venta.VentaDetalles.Add(new VentaDetalle
                {
                    Producto = new Producto
                    {
                        ProductID = item.ProductID!.Value 
                    },
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TasaIVA = VentaVM.TasaIVA
                });
            }
            return venta;
        }
        private static DateTime? CombinarFechaHora(DateTime? fecha, TimeSpan? hora)
        {
            if (!fecha.HasValue)
                return null;
            return fecha.Value.Date + (hora ?? TimeSpan.Zero);
        }
        public JsonResult OnGetFormasEnvioCliente(string customerId)
        {
            var lista = ventaService.ObtenerFormasEnvio(customerId);
            return new JsonResult(lista);
        }
        private decimal ObtenerTasaIVA(
            DateTime? fecha,
            TimeSpan? hora)
        {
            var fechaHora = CombinarFechaHora(fecha, hora);
            if (!fechaHora.HasValue)
                return 0m;
            var tasa = tasaImpuestoBLL.ObtenerTasaVigente(fechaHora.Value);
            return tasa ?? 0m;
        }
        [IgnoreAntiforgeryToken]
        public JsonResult OnPostObtenerTasaIVA(
            [FromBody] ObtenerTasaIVARequest request)
        {
            var fechaHora = CombinarFechaHora(
                request.OrderDate,
                request.OrderTime);

            if (!fechaHora.HasValue)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "Debe indicar la fecha y hora de la venta."
                });
            }

            var tasa = tasaImpuestoBLL.ObtenerTasaVigente(
                fechaHora.Value);

            if (!tasa.HasValue)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "No existe una tasa de IVA vigente para la fecha indicada."
                });
            }

            return new JsonResult(new
            {
                ok = true,
                tasaIVA = tasa.Value
            });
        }
    }
}
