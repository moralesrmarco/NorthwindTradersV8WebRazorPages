using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Tableros.Vendedores;

public class TableroControlVendedoresModel : PageModel
{
    private readonly GraficasBLL graficasBLL;

    public DataTable Anios { get; private set; } = new();
    public int AnioInicial { get; private set; } = -1;
    public int TotalAniosDisponibles { get; private set; }

    public TableroControlVendedoresModel(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NorthwindConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión NorthwindConnection.");
        graficasBLL = new GraficasBLL(connectionString);
    }

    public void OnGet()
    {
        Anios = graficasBLL.ObtenerTop10AñosDeVentas(false);
        TotalAniosDisponibles = graficasBLL.ObtenerTotalAñosConVentas();
        AnioInicial = Anios.AsEnumerable()
            .Select(row => row.Field<int>("Valor"))
            .Where(anio => anio > 0)
            .OrderByDescending(anio => anio)
            .FirstOrDefault(-1);
    }

    // Los handlers devuelven un índice visual (0-100), nunca importes ni unidades reales.
    public IActionResult OnGetVentasMensuales(int anio)
    {
        var datos = graficasBLL.ObtenerVentasMensuales(anio);
        var maximo = datos.Select(x => x.Total).DefaultIfEmpty().Max();
        return new JsonResult(datos.Select(x => new { x.Mes, x.NombreMes, indice = Normalizar(x.Total, maximo) }));
    }

    public IActionResult OnGetComparativoVentas(int anios)
    {
        var datos = graficasBLL.ObtenerVentasMensualesPorAños(Math.Max(2, anios));
        var maximo = datos.Select(x => x.Total).DefaultIfEmpty().Max();
        return new JsonResult(datos.Select(x => new { x.Year, x.Mes, x.NombreMes, indice = Normalizar(x.Total, maximo) }));
    }

    public IActionResult OnGetTopProductos(int cantidad, int anio)
    {
        cantidad = Math.Clamp(cantidad, 10, 50);
        var datos = graficasBLL.ObtenerTopProductos(cantidad, anio).AsEnumerable()
            .Select(row => new
            {
                nombreProducto = Convert.ToString(row["NombreProducto"]) ?? string.Empty,
                cantidadVendida = Convert.ToDecimal(row["CantidadVendida"])
            }).ToList();
        var maximo = datos.Select(x => x.cantidadVendida).DefaultIfEmpty().Max();
        return new JsonResult(datos.Select(x => new { x.nombreProducto, indice = Normalizar(x.cantidadVendida, maximo) }));
    }

    public IActionResult OnGetVentasPorVendedores(int anio)
    {
        var datos = graficasBLL.ObtenerVentasPorVendedores(anio)
            .Select(x => new { x.Vendedor, x.TotalVentas }).ToList();
        var maximo = datos.Select(x => x.TotalVentas).DefaultIfEmpty().Max();
        return new JsonResult(datos.Select(x => new { vendedor = x.Vendedor, indice = Normalizar(x.TotalVentas, maximo) }));
    }

    public IActionResult OnGetVentasMensualesPorVendedor(int anio)
    {
        var datos = graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio).AsEnumerable()
            .Select(row => new
            {
                vendedor = Convert.ToString(row["Vendedor"]) ?? string.Empty,
                mes = Convert.ToInt32(row["Mes"]),
                nombreMes = Convert.ToString(row["NombreMes"]) ?? string.Empty,
                totalVentas = row["TotalVentas"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalVentas"])
            }).ToList();
        var maximo = datos.Select(x => x.totalVentas).DefaultIfEmpty().Max();
        return new JsonResult(datos.Select(x => new { x.vendedor, x.mes, x.nombreMes, indice = Normalizar(x.totalVentas, maximo) }));
    }

    private static decimal Normalizar(decimal valor, decimal maximo) =>
        maximo <= 0 ? 0 : Math.Round(valor / maximo * 100, 2);
}
