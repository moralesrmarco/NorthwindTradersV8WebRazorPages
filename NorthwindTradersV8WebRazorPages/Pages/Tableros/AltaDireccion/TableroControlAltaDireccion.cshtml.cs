using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Tableros.AltaDireccion
{

    public class TableroControlAltaDireccionModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;

        public DataTable Anios { get; private set; } = new();
        public int AnioInicial { get; private set; } = -1;
        public int TotalAniosDisponibles { get; private set; }

        public TableroControlAltaDireccionModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión NorthwindConnection.");
            graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            Anios = graficasBLL.ObtenerTop10AñosDeVentas(false);
            TotalAniosDisponibles = graficasBLL.ObtenerTotalAñosConVentas();

            var aniosDisponibles = Anios.AsEnumerable()
                .Select(row => row.Field<int>("Valor"))
                .Where(anio => anio > 0)
                .OrderByDescending(anio => anio);
            AnioInicial = aniosDisponibles.FirstOrDefault(-1);
        }

        public IActionResult OnGetVentasMensuales(int anio) =>
            new JsonResult(graficasBLL.ObtenerVentasMensuales(anio));

        public IActionResult OnGetComparativoVentas(int anios) =>
            new JsonResult(graficasBLL.ObtenerVentasMensualesPorAños(Math.Max(2, anios)));

        public IActionResult OnGetTopProductos(int cantidad, int anio)
        {
            cantidad = Math.Clamp(cantidad, 10, 50);
            var datos = graficasBLL.ObtenerTopProductos(cantidad, anio);
            var productos = datos.AsEnumerable().Select(row => new
            {
                nombreProducto = Convert.ToString(row["NombreProducto"]) ?? string.Empty,
                cantidadVendida = Convert.ToInt32(row["CantidadVendida"])
            }).ToList();

            return new JsonResult(new { productos, totalUnidades = productos.Sum(x => x.cantidadVendida) });
        }

        public IActionResult OnGetVentasPorVendedores(int anio)
        {
            var vendedores = graficasBLL.ObtenerVentasPorVendedores(anio)
                .Select(x => new { vendedor = x.Vendedor, totalVentas = x.TotalVentas })
                .ToList();
            return new JsonResult(new { vendedores, totalVentas = vendedores.Sum(x => x.totalVentas) });
        }

        public IActionResult OnGetVentasMensualesPorVendedor(int anio)
        {
            var datos = graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio)
                .AsEnumerable()
                .Select(row => new
                {
                    vendedor = Convert.ToString(row["Vendedor"]) ?? string.Empty,
                    mes = Convert.ToInt32(row["Mes"]),
                    nombreMes = Convert.ToString(row["NombreMes"]) ?? string.Empty,
                    totalVentas = row["TotalVentas"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalVentas"])
                });
            return new JsonResult(datos);
        }
    }
}