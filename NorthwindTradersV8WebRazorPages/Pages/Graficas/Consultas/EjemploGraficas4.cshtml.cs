using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class EjemploGraficas4Model : PageModel
    {
        private readonly GraficasBLL graficasBLL;
        public DataTable AñosVentas { get; set; } = new DataTable();
        [BindProperty(SupportsGet = true)]
        public int Anio { get; set; }
        public EjemploGraficas4Model(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not found");

            graficasBLL = new GraficasBLL(connectionString);
        }
        public void OnGet()
        {
            AñosVentas = graficasBLL.ObtenerTop10AñosDeVentas(false);
        }
        public IActionResult OnGetVentasMensualesPorVendedores(int anio)
        {
            DataTable dt =
                graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row["Vendedor"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["Vendedor"]),

                    mes = row["Mes"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Mes"]),

                    nombreMes = row["NombreMes"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["NombreMes"]),

                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            return new JsonResult(datos);
        }
    }
}