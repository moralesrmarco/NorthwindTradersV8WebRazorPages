using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class VentasPorVendedoresPorAnioBarrasModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;
        public DataTable Años { get; set; } = new DataTable();
        public int AñoActual { get; set; } = DateTime.Today.Year;
        public VentasPorVendedoresPorAnioBarrasModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            graficasBLL = new GraficasBLL(connectionString);
            Años = graficasBLL.ObtenerTop10AñosDeVentas(false);
        }
        public IActionResult OnGetVentasMensualesPorVendedoresPorAnio(int anio)
        {
            DataTable dt =
                graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

            var datos = dt.AsEnumerable()
                    .Select(row => new
                    {
                        Vendedor = row["Vendedor"] == DBNull.Value
                            ? string.Empty
                            : Convert.ToString(row["Vendedor"]),

                        Mes = row["Mes"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(row["Mes"]),

                        NombreMes = row["NombreMes"] == DBNull.Value
                            ? string.Empty
                            : Convert.ToString(row["NombreMes"]),

                        TotalVentas = row["TotalVentas"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(row["TotalVentas"])
                    })
                    .ToList();

            return new JsonResult(datos);
        }
    }
}