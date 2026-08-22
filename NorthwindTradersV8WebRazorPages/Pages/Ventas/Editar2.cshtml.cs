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
    public class Editar2Model : PageModel
    {
        private readonly VentaBLL ventaBLL;
        private readonly VentaDetalleBLL ventaDetalleBLL;
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
        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> Transportistas { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Productos { get; set; } = new();
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
        public bool VentaTieneDetalles { get; set; }
        public Editar2Model(IConfiguration configuration)
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
        public IActionResult OnGet(int id)
        {
            var venta = ventaBLL.ObtenerVentaPorId2(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                BloquearEdicion = true;
                return Page();
            }
            CargarCombos();
            decimal tasaIVA = 0m;
            if (venta.OrderDate.HasValue)
            {
                var tasa = venta.OrderDate.HasValue
                    ? tasaImpuestoBLL.ObtenerTasaVigente(venta.OrderDate.Value)
                    : null;
                if (tasa.HasValue)
                    tasaIVA = tasa.Value;
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
                RowVersion = venta.RowVersion,
                TasaIVA = tasaIVA
            };
            var detalles = ventaDetalleBLL.ObtenerDetallesPorVentaId(id);
            VentaTieneDetalles = detalles.Any();
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
        private VentaTotalesViewModel CalcularTotalesVenta(
            List<VentaDetalleViewModel> lista)
        {
            var detalles = lista
                .Select(x => x.ToVentaDetalle())
                .ToList();

            return new VentaTotalesViewModel
            {
                NumeroProductos = detalles.Count,

                TotalUnidades =
                    detalles.Sum(x => x.Quantity),

                TotalImporteConIVA =
                    detalles.Sum(x => x.SubtotalDelImporteConIVAIncluido),

                TotalDescuento =
                    detalles.Sum(x => x.SubtotalDelAhorroTotalDespuesDescuento),

                TotalImporteConDescuento =
                    detalles.Sum(x => x.SubtotalDelImporteConIVAConDescuento),

                TotalImporteSinIVA =
                    detalles.Sum(x => x.SubtotalDelImporteSinIVAConDescuento),

                TotalIVA =
                    detalles.Sum(x => x.SubtotalIVADespuesDelDescuento),

                Total =
                    detalles.Sum(x => x.Subtotal)
            };
        }
        private static List<VentaDetalleViewModel> CrearListaViewModel(
            List<VentaDetalle> lista)
        {
            return lista.Select(x => new VentaDetalleViewModel
            {
                // =============================================
                // DATOS BASE
                // =============================================

                CategoriaID = x.Producto.Categoria?.CategoryID,

                ProductID = x.Producto.ProductID,

                ProductName = x.Producto.ProductName,

                Quantity = x.Quantity,

                UnitPrice = x.UnitPrice,

                Discount = x.Discount,

                TasaIVA = x.TasaIVA,

                RowVersion = x.RowVersion == null
                    ? null
                    : Convert.ToBase64String(x.RowVersion),


                // =============================================
                // RESULTADOS CALCULADOS EN VentaDetalle
                // =============================================

                PrecioBaseSinIva =
                    x.PrecioBaseSinIva,

                PrecioPorUnidadSinIVASinDescuento =
                    x.PrecioPorUnidadSinIVASinDescuento,

                IVADelPrecioPorUnidadSinDescuento =
                    x.IVADelPrecioPorUnidadSinDescuento,

                PrecioPorUnidadConIVADespuesDescuento =
                    x.PrecioPorUnidadConIVADespuesDescuento,

                IVADelPrecioporUnidadDespuesDescuento =
                    x.IVADelPrecioporUnidadDespuesDescuento,

                PrecioPorUnidadSinIVADespuesDescuento =
                    x.PrecioPorUnidadSinIVADepuesDescuento,

                AhorroPorUnidadSinIVA =
                    x.AhorroPorUnidadSinIVA,

                AhorroEnIVAPorUnidadDespuesDescuento =
                    x.AhorroEnIVAPorUnidadDespuesDescuento,

                AhorroTotalPorUnidadConIVA =
                    x.AhorroTotalPorUnidadConIVA,

                TasaDescuentoPorcentaje =
                    x.TasaDescuentoPorcentaje,

                TasaIVAPorcentaje =
                    x.TasaIVAPorcentaje,


                // =============================================
                // SUBTOTALES
                // =============================================

                SubtotalDelImporteConIVAIncluido =
                    x.SubtotalDelImporteConIVAIncluido,

                SubtotalDelImporteSinIVASinDescuento =
                    x.SubtotalDelImporteSinIVASinDescuento,

                SubtotalDelImporteDelIVASinDescuento =
                    x.SubtotalDelImporteDelIVASinDescuento,

                SubtotalDelImporteConIVAConDescuento =
                    x.SubtotalDelImporteConIVAConDescuento,

                SubtotalDelImporteSinIVAConDescuento =
                    x.SubtotalDelImporteSinIVAConDescuento,

                SubtotalIVADespuesDelDescuento =
                    x.SubtotalIVADespuesDelDescuento,

                SubtotalDelAhorroSinIvaDespuesDescuento =
                    x.SubtotalDelAhorroSinIvaDespuesDescuento,

                SubtotalDelAhorroEnIVADespuesDescuento =
                    x.SubtotalDelAhorroEnIVADespuesDescuento,

                SubtotalDelAhorroTotalDespuesDescuento =
                    x.SubtotalDelAhorroTotalDespuesDescuento,

                Subtotal =
                    x.Subtotal

            }).ToList();
        }
        public IActionResult OnPostActualizarEncabezado(
            [FromBody] ActualizarEncabezadoRequest request)
        {
            try
            {
                var resultado =
                    ventaBLL.ActualizarEncabezado2(request);
                switch (resultado.Codigo)
                {
                    case 1:
                        return new JsonResult(new
                        {
                            ok = true,
                            rowVersion = resultado.RowVersion == null
                                ? null
                                : Convert.ToBase64String(
                                    resultado.RowVersion)
                        });

                    case -1:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -1,
                            mensaje = "La venta fue eliminada previamente por otro usuario."
                        });

                    case -2:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -2,
                            mensaje = "La venta fue modificada previamente por otro usuario."
                        });

                    case -10:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -10,
                            mensaje = "No se puede modificar la fecha de la venta porque ya existen detalles asociados. La fecha determina la tasa de IVA aplicada a la venta."
                        });

                    case -11:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -11,
                            mensaje = "No existe una tasa de IVA vigente para la fecha de la venta indicada."
                        });

                    case -99:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -99,
                            mensaje = "Ocurrió un error al actualizar la venta."
                        });

                    default:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = resultado.Codigo,
                            mensaje = "No se pudo actualizar la venta."
                        });
                }
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
                var resultado =
                    ventaBLL.ActualizarEnvio2(request);
                switch (resultado.Codigo)
                {
                    case 1:
                        return new JsonResult(new
                        {
                            ok = true,
                            rowVersion = resultado.RowVersion == null
                                ? null
                                : Convert.ToBase64String(
                                    resultado.RowVersion)
                        });

                    case -1:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -1,
                            mensaje = "La venta fue eliminada previamente por otro usuario."
                        });

                    case -2:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -2,
                            mensaje = "La venta fue modificada previamente por otro usuario."
                        });

                    case -99:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = -99,
                            mensaje = "Ocurrió un error al actualizar la venta."
                        });

                    default:
                        return new JsonResult(new
                        {
                            ok = false,
                            codigo = resultado.Codigo,
                            mensaje = "No se pudo actualizar la venta."
                        });
                }
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
        public JsonResult OnGetFormasEnvioCliente(string customerId)
        {
            var lista = ventaService.ObtenerFormasEnvio(customerId);
            return new JsonResult(lista);
        }

        public JsonResult OnGetUltimaInformacionEnvio(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return new JsonResult(null);

            var informacion =
                ventaService.ObtenerUltimaInformacionDeEnvio(customerId);
            return new JsonResult(informacion);
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
        public JsonResult OnPostCalcularDetalle(
            [FromBody] VentaDetalleViewModel detalleVM)
        {
            var detalle = detalleVM.ToVentaDetalle();

            var resultado = new VentaDetalleViewModel
            {
                CategoriaID = detalleVM.CategoriaID,
                ProductID = detalleVM.ProductID,
                ProductName = detalleVM.ProductName,

                UnitPrice = detalle.UnitPrice,
                UnitsInStock = detalleVM.UnitsInStock,
                Quantity = detalle.Quantity,
                Discount = detalle.Discount,
                TasaIVA = detalle.TasaIVA,

                // =============================================
                // RESULTADOS CENTRALIZADOS EN VentaDetalle
                // =============================================

                PrecioBaseSinIva =
                    detalle.PrecioBaseSinIva,

                PrecioPorUnidadSinIVASinDescuento =
                    detalle.PrecioPorUnidadSinIVASinDescuento,

                IVADelPrecioPorUnidadSinDescuento =
                    detalle.IVADelPrecioPorUnidadSinDescuento,

                PrecioPorUnidadConIVADespuesDescuento =
                    detalle.PrecioPorUnidadConIVADespuesDescuento,

                PrecioPorUnidadSinIVADespuesDescuento =
                    detalle.PrecioPorUnidadSinIVADepuesDescuento,

                IVADelPrecioporUnidadDespuesDescuento =
                    detalle.IVADelPrecioporUnidadDespuesDescuento,

                AhorroPorUnidadSinIVA =
                    detalle.AhorroPorUnidadSinIVA,

                AhorroEnIVAPorUnidadDespuesDescuento =
                    detalle.AhorroEnIVAPorUnidadDespuesDescuento,

                AhorroTotalPorUnidadConIVA =
                    detalle.AhorroTotalPorUnidadConIVA,

                TasaDescuentoPorcentaje =
                    detalle.TasaDescuentoPorcentaje,

                TasaIVAPorcentaje =
                    detalle.TasaIVAPorcentaje,


                // =============================================
                // SUBTOTALES
                // =============================================

                SubtotalDelImporteConIVAIncluido =
                    detalle.SubtotalDelImporteConIVAIncluido,

                SubtotalDelImporteSinIVASinDescuento =
                    detalle.SubtotalDelImporteSinIVASinDescuento,

                SubtotalDelImporteDelIVASinDescuento =
                    detalle.SubtotalDelImporteDelIVASinDescuento,

                SubtotalDelImporteConIVAConDescuento =
                    detalle.SubtotalDelImporteConIVAConDescuento,

                SubtotalDelImporteSinIVAConDescuento =
                    detalle.SubtotalDelImporteSinIVAConDescuento,

                SubtotalIVADespuesDelDescuento =
                    detalle.SubtotalIVADespuesDelDescuento,

                SubtotalDelAhorroSinIvaDespuesDescuento =
                    detalle.SubtotalDelAhorroSinIvaDespuesDescuento,

                SubtotalDelAhorroEnIVADespuesDescuento =
                    detalle.SubtotalDelAhorroEnIVADespuesDescuento,

                SubtotalDelAhorroTotalDespuesDescuento =
                    detalle.SubtotalDelAhorroTotalDespuesDescuento,

                Subtotal =
                    detalle.Subtotal
            };

            return new JsonResult(resultado);
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

                var resultado = ventaDetalleBLL.ActualizarDetalle(detalle);

                if (resultado.Codigo != 1)
                    return ResultadoActualizarDetalleError(
                        resultado.Codigo);
                VentaVM.RowVersion = resultado.RowVersion;
                var lista = ventaDetalleBLL
                    .ObtenerDetallesPorVentaId(request.OrderID);
                var listaViewModel = CrearListaViewModel(lista);
                var totales =
                    CalcularTotalesVenta(listaViewModel); 
                return new JsonResult(new
                {
                    ok = true,
                    rowVersion = Convert.ToBase64String(
                        resultado.RowVersion!),
                    lista = listaViewModel,
                    totales
                });
            }
            catch (Exception ex)
            {
                return ErrorDetalle(ex.Message);
            }
        }
        public JsonResult OnPostAgregarDetalle([FromBody] AgregarDetalleRequest request)
        {
            try
            {
                if (request == null || request.Detalle == null ||
                    !request.Detalle.ProductID.HasValue)
                {
                    return new JsonResult(new
                    {
                        ok = false,
                        codigo = 0,
                        mensaje = "Debe seleccionar un producto."
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
                    // LA TASA YA VIENE DE LA VENTA
                    TasaIVA = request.Detalle.TasaIVA
                };
                var resultado = ventaDetalleBLL.InsertarDetalle(detalle);
                if (resultado.Codigo != 1)
                    return ResultadoAgregarDetalleError(resultado.Codigo);
                VentaVM.RowVersion = resultado.RowVersion;
                // Obtener datos actuales desde BD
                var lista = ventaDetalleBLL
                            .ObtenerDetallesPorVentaId(request.OrderID);
                var listaViewModel = CrearListaViewModel(lista);
                var totales = CalcularTotalesVenta(listaViewModel);
                return new JsonResult(new
                {
                    ok = true,
                    rowVersion = Convert.ToBase64String(
                        resultado.RowVersion!),
                    lista = listaViewModel,
                    totales
                });
            }
            catch
            {
                return new JsonResult(new
                {
                    ok = false,
                    codigo = -99,
                    mensaje = "Ocurrió un error inesperado al insertar el detalle."
                });
            }
        }
        private JsonResult ResultadoActualizarDetalleError(int codigo)
        {
            string mensaje = codigo switch
            {
                -1 => "El detalle fue eliminado por otro usuario. Recargue la venta para actualizar los datos.",
                -2 => "El detalle fue modificado por otro usuario. Recargue la venta para actualizar los datos.",
                -3 => "La venta fue eliminada por otro usuario.",
                -4 => "La venta fue modificada previamente por otro usuario. Recargue la venta para actualizar los datos.",
                -5 => "La cantidad del detalle no es válida.",
                -6 => "No hay inventario suficiente para actualizar el detalle.",
                -7 => "El inventario resultante excede el límite permitido.",
                -8 => "El inventario resultaría negativo.",
                -99 => "Ocurrió un error inesperado al actualizar el detalle.",
                _ => "No se pudo actualizar el detalle de la venta."
            };
            return new JsonResult(new
            {
                ok = false,
                codigo,
                mensaje
            });
        }
        private JsonResult ResultadoAgregarDetalleError(int codigo)
        {
            string mensaje = codigo switch
            {
                -1 => "El producto ya existe en el detalle de la venta.",
                -3 => "La venta fue eliminada por otro usuario.",
                -4 => "La venta fue modificada previamente por otro usuario. Recargue la venta para actualizar los datos.",
                -5 => "No se pudo insertar el detalle de la venta.",
                -6 => "No hay existencias suficientes para agregar la cantidad solicitada.",
                -7 => "El inventario resultante excede el límite permitido.",
                -8 => "El inventario resultante no puede ser negativo.",
                -10 => "No existe tasa de IVA para la fecha de la venta.",
                -99 => "Ocurrió un error inesperado al insertar el detalle.",
                _ => "No se pudo insertar el detalle de la venta."
            };

            return new JsonResult(new
            {
                ok = false,
                codigo,
                mensaje
            });
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
                        bloquearEdicionPorConcurrencia = true,
                        message = "El detalle fue eliminado por otro usuario. Recargue la venta para actualizar los datos."
                    });
                }
                if (detalle.RowVersion == null ||
                    !detalle.RowVersion.SequenceEqual(rowVersion))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        bloquearEdicionPorConcurrencia = true,
                        message = "El detalle fue modificado por otro usuario. Recargue la venta para actualizar los datos."
                    });
                }

                var producto = productoService
                    .ObtenerProductoCostoEInventario(request.ProductID);
                if (producto == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        bloquearEdicionPorConcurrencia = true,
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
        public JsonResult OnPostEliminarDetalle(
            [FromBody] EliminarDetalleRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorDetalle(
                        "Datos inválidos.");
                if (request.OrderID <= 0)
                    return ErrorDetalle(
                        "El OrderID no es válido.");
                if (request.ProductID <= 0)
                    return ErrorDetalle(
                        "El ProductID no es válido.");
                if (string.IsNullOrWhiteSpace(request.RowVersion))
                    return ErrorDetalle(
                        "No se recibió la RowVersion del detalle.");
                if (string.IsNullOrWhiteSpace(request.VentaRowVersion))
                    return ErrorDetalle(
                        "No se recibió la RowVersion de la venta.");
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
                    RowVersion = Convert.FromBase64String(
                        request.RowVersion)
                };
                var resultado =
                    ventaDetalleBLL.EliminarDetalle(detalle);
                if (resultado.Codigo != 1)
                    return ResultadoEliminarDetalleError(
                        resultado.Codigo);
                VentaVM.RowVersion =
                    resultado.RowVersion;
                var lista = ventaDetalleBLL
                    .ObtenerDetallesPorVentaId(
                        request.OrderID);
                var listaViewModel =
                    CrearListaViewModel(lista);
                var totales =
                    CalcularTotalesVenta(
                        listaViewModel);
                return new JsonResult(new
                {
                    ok = true,
                    rowVersion = Convert.ToBase64String(
                        resultado.RowVersion!),
                    lista = listaViewModel,
                    totales
                });
            }
            catch (Exception ex)
            {
                return ErrorDetalle(
                    ex.Message);
            }
        }
        private JsonResult ResultadoEliminarDetalleError(
            int codigo)
        {
            string mensaje = codigo switch
            {
                -1 => "El detalle fue eliminado por otro usuario. Recargue la venta para actualizar los datos.",
                -2 => "El detalle fue modificado por otro usuario. Recargue la venta para actualizar los datos.",
                -3 => "La venta fue eliminada por otro usuario.",
                -4 => "La venta fue modificada previamente por otro usuario. Recargue la venta para actualizar los datos.",
                -5 => "La cantidad del detalle no es válida.",
                -7 => "El inventario resultante excede el límite permitido.",
                -8 => "El inventario resultaría negativo.",
                -99 => "Ocurrió un error inesperado al eliminar el detalle.",
                _ => "No se pudo eliminar el detalle de la venta."
            };

            return new JsonResult(new
            {
                ok = false,
                codigo,
                mensaje
            });
        }
        private JsonResult ErrorDetalle(
            string mensaje,
            int codigo = -99)
        {
            return new JsonResult(new
            {
                ok = false,
                codigo,
                mensaje
            });
        }
    }
}
