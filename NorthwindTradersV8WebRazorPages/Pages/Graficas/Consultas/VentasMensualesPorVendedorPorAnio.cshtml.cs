using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Graficas.Consultas
{
    public class VentasMensualesPorVendedorPorAnioModel : PageModel
    {
        private readonly GraficasBLL graficasBLL;
        public DataTable Años { get; set; } = new DataTable();
        public int AñoActual { get; set; }
        public VentasMensualesPorVendedorPorAnioModel(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            graficasBLL = new GraficasBLL(connectionString);
        }

        public void OnGet()
        {
            Años = graficasBLL.ObtenerTop10AñosDeVentas(false);
            AñoActual = DateTime.Now.Year;
        }
        public JsonResult OnGetVentasMensualesPorVendedorPorAnio(int year)
        {
            var dt =
                graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(year);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row.Field<string>("Vendedor"),
                    nombreMes = row.Field<string>("NombreMes"),
                    totalVentas =
                        row["TotalVentas"] != DBNull.Value
                            ? Convert.ToDecimal(row["TotalVentas"])
                            : 0m
                })
                .ToList();

            return new JsonResult(datos);
        }
    }
}