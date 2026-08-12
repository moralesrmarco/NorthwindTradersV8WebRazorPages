using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
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
