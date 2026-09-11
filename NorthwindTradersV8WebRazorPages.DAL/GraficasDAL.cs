using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class GraficasDAL
    {
        private readonly string connectionString;
        public GraficasDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerTop10AñosDeVentas()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerTop10Años", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los años de ventas" + ex.Message);
            }
            return dt;
        }
        public List<DtoVentasMensuales> ObtenerVentasMensuales(int year)
        {
            var lista = new List<DtoVentasMensuales>();
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentasObtenerMensuales", cn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        int colMes = reader.GetOrdinal("Mes");
                        int colNombreMes = reader.GetOrdinal("NombreMes");
                        int colTotal = reader.GetOrdinal("Total");
                        while (reader.Read())
                        {
                            int mes = reader.IsDBNull(colMes) ? 0 : Convert.ToInt32(reader.GetValue(colMes));
                            string nombre = reader.IsDBNull(colNombreMes) ? string.Empty : reader.GetString(colNombreMes);
                            decimal total = reader.IsDBNull(colTotal) ? 0m : Convert.ToDecimal(reader.GetValue(colTotal));
                            lista.Add(new DtoVentasMensuales { Mes = mes, NombreMes = nombre, Total = total });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas mensuales: " + ex.Message);
            }
            return lista;
        }

    }
}
