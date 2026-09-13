using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class GraficasBLL
    {
        private readonly GraficasDAL graficasDAL;
        public GraficasBLL(string connectionString)
        {
            graficasDAL = new GraficasDAL(connectionString);
        }
        public DataTable ObtenerTop10AñosDeVentas(bool conFilaSeleccione = true)
        {
            DataTable dtDb = graficasDAL.ObtenerTop10AñosDeVentas();
            DataTable dt = new DataTable();
            dt.Columns.Add("Texto", typeof(string));
            dt.Columns.Add("Valor", typeof(int));
            if (conFilaSeleccione)
                dt.Rows.Add("»--- Seleccione ---«", 0);
            dt.Rows.Add("Todos los años", -1);
            foreach (DataRow row in dtDb.Rows)
            {
                int anio = Convert.ToInt32(row["YearOrderDate"]);
                dt.Rows.Add(anio.ToString(), anio);
            }
            return dt;
        }
        public List<DtoVentasMensuales> ObtenerVentasMensuales(int year)
        {
            return graficasDAL.ObtenerVentasMensuales(year);
        }
        public int ObtenerTotalAñosConVentas()
        {
            return graficasDAL.ObtenerTotalAñosConVentas();
        }
        public List<DtoVentasMensualesPorAños> ObtenerVentasMensualesPorAños(int years)
        {
            return graficasDAL.ObtenerVentasMensualesPorAños(years);
        }
        public DataTable ObtenerTopProductos(int cantidad, int anio)
        {
            return graficasDAL.ObtenerTopProductos(cantidad, anio);
        }
        public List<(string Vendedor, decimal TotalVentas)> ObtenerVentasPorVendedores(int anio = 0)
        {
            return graficasDAL.ObtenerVentasPorVendedores(anio);
        }
        public DataTable ObtenerVentasMensualesPorVendedoresPorAño(int anio)
        {
            return graficasDAL.ObtenerVentasMensualesPorVendedoresPorAño(anio);
        }
        public DataTable ObtenerTopProductosRpt(int cantidad, int anio)
        {
            return graficasDAL.ObtenerTopProductosRpt(cantidad, anio);
        }


    }
}
