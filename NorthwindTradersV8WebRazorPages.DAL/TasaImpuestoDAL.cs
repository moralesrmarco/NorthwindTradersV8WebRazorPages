using Microsoft.Data.SqlClient;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class TasaImpuestoDAL
    {
        private readonly string connectionString;
        public TasaImpuestoDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public decimal? ObtenerTasaVigente(DateTime fecha)
        {
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd =
                new("SpTasaImpuestoObtenerVigente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2)
                .Value = fecha;
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return Convert.ToDecimal(dr["Tasa"]);
            }
            return null;
        }
    }
}
