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

    }
}
