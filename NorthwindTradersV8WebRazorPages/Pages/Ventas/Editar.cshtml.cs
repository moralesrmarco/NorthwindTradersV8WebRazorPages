using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using NorthwindTradersV8WebRazorPages.ViewModels;
using System.Text.Json;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class EditarModel : PageModel
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
        public bool BloquearEdicion { get; set; }
        public EditarModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            ventaDetalleBLL = new VentaDetalleBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
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
        public IActionResult OnGet(int id)
        {
            CargarCombos();
            var venta = ventaBLL.ObtenerVentaPorId2(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                BloquearEdicion = true;
                return Page();
            }
            VentaVM = new VentaInsertarViewModel
            {
                OrderID = venta.OrderID,
                CustomerID = venta.Cliente.CustomerID,
                EmployeeID = venta.Empleado.EmployeeID,
                OrderDate = venta.OrderDate,
                OrderTime = venta.OrderDate?.TimeOfDay,
                RequiredDate = venta.RequiredDate,
                RequiredTime = venta.RequiredDate?.TimeOfDay,
                ShippedDate = venta.ShippedDate,
                ShippedTime = venta.ShippedDate?.TimeOfDay,
                ShipVia = venta.Transportista.ShipperID,
                ShipName = venta.ShipName,
                ShipAddress = venta.ShipAddress,
                ShipCity = venta.ShipCity,
                ShipRegion = venta.ShipRegion,
                ShipPostalCode = venta.ShipPostalCode,
                ShipCountry = venta.ShipCountry,
                Freight = venta.Freight ?? 0m,
                RowVersion = venta.RowVersion
            };
            var detalles = ventaDetalleBLL.ObtenerDetallesPorVentaId(id);
            Detalles = CrearListaViewModel(detalles);
            Totales = CalcularTotalesVenta(Detalles);
            return Page();
        }
        public void CargarCombos()
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
                Value = t.Value,
                Text = t.Text,
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
        public IActionResult OnPostActualizarEncabezado(
            [FromBody] ActualizarEncabezadoRequest request)
        {
            try
            {
                if (VentaVM.OrderDate.HasValue)
                {
                    VentaVM.OrderDate = VentaVM.OrderDate.Value.Date
                        .Add(VentaVM.OrderTime ?? TimeSpan.Zero);
                }
                if (VentaVM.RequiredDate.HasValue)
                {
                    VentaVM.RequiredDate = VentaVM.RequiredDate.Value.Date
                        .Add(VentaVM.RequiredTime ?? TimeSpan.Zero);
                }
                if (VentaVM.ShippedDate.HasValue)
                {
                    VentaVM.ShippedDate = VentaVM.ShippedDate.Value.Date
                        .Add(VentaVM.ShippedTime ?? TimeSpan.Zero);
                }
                var nuevaRowVersion =
                    ventaBLL.ActualizarEncabezado(request);
                return new JsonResult(new
                {
                    ok = true,
                    rowVersion = Convert.ToBase64String(nuevaRowVersion)
                });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = ex.Message
                });
            }
        }
        [ValidateAntiForgeryToken]
        public IActionResult OnPostActualizarEnvio(
            [FromBody] ActualizarEnvioRequest request)
        {
            try
            {
                var nuevaRowVersion =
                    ventaBLL.ActualizarEnvio(request);
                return new JsonResult(new
                {
                    ok = true,
                    rowVersion = Convert.ToBase64String(nuevaRowVersion)
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = ex.Message
                });
            }
        }
        public JsonResult OnGetFormasEnvioCliente(string customerId)
        {
            var lista = ventaService.ObtenerFormasEnvio(customerId);
            return new JsonResult(lista);
        }
        public JsonResult OnPostObtenerDetalleEditar(
            [FromBody] ObtenerDetalleEditarRequest request)
        {
            try
            {
                if (request == null || request.OrderID <= 0 || request.ProductID <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Los datos del detalle no son válidos."
                    });
                }
                if (string.IsNullOrWhiteSpace(request.RowVersion))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No se recibió la RowVersion del detalle."
                    });
                }

                var rowVersion = Convert.FromBase64String(request.RowVersion);
                var detalle = ventaDetalleBLL
                    .ObtenerDetallesPorVentaId(request.OrderID)
                    .FirstOrDefault(d => d.Producto.ProductID == request.ProductID);
                if (detalle == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El detalle ya no existe."
                    });
                }
                if (detalle.RowVersion == null ||
                    !detalle.RowVersion.SequenceEqual(rowVersion))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El detalle fue modificado por otro usuario."
                    });
                }

                var producto = productoService
                    .ObtenerProductoCostoEInventario(request.ProductID);
                if (producto == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El producto ya no existe."
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    detalle = new VentaDetalleViewModel
                    {
                        CategoriaID = detalle.Producto.Categoria?.CategoryID,
                        ProductID = detalle.Producto.ProductID,
                        ProductName = detalle.Producto.ProductName,
                        UnitPrice = detalle.UnitPrice,
                        UnitsInStock = producto.UnitsInStock,
                        Quantity = detalle.Quantity,
                        Discount = detalle.Discount,
                        TasaIVA = detalle.TasaIVA,
                        RowVersion = Convert.ToBase64String(detalle.RowVersion)
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        public JsonResult OnPostAgregarDetalle([FromBody] AgregarDetalleRequest request)
        {
            try
            {
                if (!request.Detalle.ProductID.HasValue)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Debe seleccionar un producto x."
                    });
                }
                var detalle = new VentaDetalle
                {
                    Venta = new Venta
                    {
                        OrderID = request.OrderID,
                        RowVersion = string.IsNullOrEmpty(request.RowVersion)
                            ? null
                            : Convert.FromBase64String(request.RowVersion)
                    },
                    Producto = new Producto
                    {
                        ProductID = request.Detalle.ProductID.Value
                    },
                    UnitPrice = request.Detalle.UnitPrice,
                    Quantity = request.Detalle.Quantity,
                    Discount = request.Detalle.Discount,
                    TasaIVA = request.Detalle.TasaIVA
                };
                ventaDetalleBLL.InsertarDetalle(detalle);
                // El DAL ya actualizó este valor después del SP
                VentaVM.RowVersion = detalle.Venta.RowVersion;
                // Obtener datos actuales desde BD
                var lista = ventaDetalleBLL
                            .ObtenerDetallesPorVentaId(request.OrderID);
                var listaViewModel = CrearListaViewModel(lista);
                var totales = CalcularTotalesVenta(listaViewModel);
                return new JsonResult(new
                {
                    success = true,
                    rowVersion = Convert.ToBase64String(
                        detalle.Venta.RowVersion),
                    lista = listaViewModel,
                    totales
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        public JsonResult OnGetProductosPorCategoria(int categoriaId)
        {
            var productos = productoService.ObtenerProductosPorCategoriaCbo(categoriaId);
            return new JsonResult(productos);
        }
        public JsonResult OnGetProductoCostoEInventario(int productId)
        {
            var producto = productoService.ObtenerProductoCostoEInventario(productId);
            return new JsonResult(producto);
        }
        public JsonResult OnPostCalcularDetalle([FromBody] VentaDetalleViewModel detalle)
        {
            return new JsonResult(detalle);
        }
        public JsonResult OnPostActualizarDetalle(
            [FromBody] ActualizarDetalleRequest request)
        {
            try
            {
                if (request == null || request.OrderID <= 0)
                    return ErrorDetalle("El OrderID no es válido.");
                if (request.ProductID <= 0)
                    return ErrorDetalle("El ProductID no es válido.");
                if (request.Quantity <= 0)
                    return ErrorDetalle("La cantidad debe ser mayor que cero.");
                if (request.Discount < 0 || request.Discount > 0.95m)
                    return ErrorDetalle("El descuento debe estar entre 0 y 95.00%.");
                if (string.IsNullOrWhiteSpace(request.VentaDetalleRowVersion))
                    return ErrorDetalle("No se recibió la RowVersion del detalle.");
                if (string.IsNullOrWhiteSpace(request.VentaRowVersion))
                    return ErrorDetalle("No se recibió la RowVersion de la venta.");

                var detalle = new VentaDetalle
                {
                    Venta = new Venta
                    {
                        OrderID = request.OrderID,
                        RowVersion = Convert.FromBase64String(
                            request.VentaRowVersion)
                    },
                    Producto = new Producto
                    {
                        ProductID = request.ProductID
                    },
                    Quantity = request.Quantity,
                    Discount = request.Discount,
                    RowVersion = Convert.FromBase64String(
                        request.VentaDetalleRowVersion)
                };

                ventaDetalleBLL.ActualizarDetalle(detalle);

                var ventaActualizada = ventaBLL.ObtenerVentaPorId2(
                    request.OrderID);
                if (ventaActualizada?.RowVersion == null)
                    throw new Exception(
                        "No se pudo obtener la RowVersion actualizada de la venta.");

                var lista = ventaDetalleBLL
                    .ObtenerDetallesPorVentaId(request.OrderID);
                var listaViewModel = CrearListaViewModel(lista);
                var totales = CalcularTotalesVenta(listaViewModel);

                return new JsonResult(new
                {
                    success = true,
                    rowVersion = Convert.ToBase64String(
                        ventaActualizada.RowVersion),
                    lista = listaViewModel,
                    totales
                });
            }
            catch (Exception ex)
            {
                return ErrorDetalle(ex.Message);
            }
        }
        public JsonResult OnPostEliminarDetalle(
            [FromBody] EliminarDetalleRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Datos inválidos."
                    });
                }
                if (request.OrderID <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El OrderID no es válido."
                    });
                }
                if (request.ProductID <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El ProductID no es válido."
                    });
                }
                if (string.IsNullOrWhiteSpace(request.RowVersion))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No se recibió la RowVersion del detalle."
                    });
                }
                if (string.IsNullOrWhiteSpace(request.VentaRowVersion))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No se recibió la RowVersion de la venta."
                    });
                }
                var detalle = new VentaDetalle
                {
                    Venta = new Venta
                    {
                        OrderID = request.OrderID,

                        RowVersion = Convert.FromBase64String(request.VentaRowVersion)
                    },
                    Producto = new Producto
                    {
                        ProductID = request.ProductID
                    },
                    RowVersion = Convert.FromBase64String(request.RowVersion)
                };
                ventaDetalleBLL.EliminarDetalle(detalle);
                // Actualizar RowVersion de la venta
                VentaVM.RowVersion =
                    detalle.Venta.RowVersion;
                // Obtener datos actuales desde BD
                var lista = ventaDetalleBLL
                    .ObtenerDetallesPorVentaId(request.OrderID);
                var listaViewModel = CrearListaViewModel(lista);
                var totales =
                    CalcularTotalesVenta(listaViewModel);
                return new JsonResult(new
                {
                    success = true,
                    rowVersion = Convert.ToBase64String(
                        detalle.Venta.RowVersion),
                    lista = listaViewModel,
                    totales
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        private JsonResult ErrorDetalle(string message)
        {
            return new JsonResult(new
            {
                success = false,
                message
            });
        }
        private static List<VentaDetalleViewModel> CrearListaViewModel(
            List<VentaDetalle> lista)
        {
            return lista.Select(x => new VentaDetalleViewModel
            {
                CategoriaID = x.Producto.Categoria?.CategoryID,
                ProductID = x.Producto.ProductID,
                ProductName = x.Producto.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Discount = x.Discount,
                TasaIVA = x.TasaIVA,
                RowVersion = x.RowVersion == null
                    ? null
                    : Convert.ToBase64String(x.RowVersion)
            }).ToList();
        }
    }
}
