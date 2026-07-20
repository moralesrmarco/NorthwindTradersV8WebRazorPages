using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class VentaDAL
    {
        private readonly string connectionString;
        public VentaDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerVentasPaginadas(int pageIndex, int pageSize, out int totalRegistros)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SpVentasObtenerPaginadas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageIndex", pageIndex);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    using (var dap = new SqlDataAdapter(command))
                    {
                        var ds = new DataSet();
                        dap.Fill(ds);
                        totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                        return ds.Tables[1]; 
                    }
                }
            }
        }
        public List<VentaDto> ObtenerVentasBuscadasPaginadas(VentasBuscarDto filtro, int pageIndex, int pageSize, out int totalRegistros)
        {
            var ventas = new List<VentaDto>();
            totalRegistros = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("usp_ObtenerVentasBuscadasPaginadas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdIni", (object?)filtro.IdIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdFin", (object?)filtro.IdFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cliente", (object?)filtro.Cliente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVenta", filtro.FVenta);
                    command.Parameters.AddWithValue("@FVentaIni", (object?)filtro.FVentaIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVentaFin", (object?)filtro.FVentaFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVentaNull", filtro.FVentaNull);
                    command.Parameters.AddWithValue("@FRequerido", filtro.FRequerido);
                    command.Parameters.AddWithValue("@FRequeridoIni", (object?)filtro.FRequeridoIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FRequeridoFin", (object?)filtro.FRequeridoFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FRequeridoNull", filtro.FRequeridoNull);
                    command.Parameters.AddWithValue("@FEnvio", filtro.FEnvio);
                    command.Parameters.AddWithValue("@FEnvioIni", (object?)filtro.FEnvioIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FEnvioFin", (object?)filtro.FEnvioFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FEnvioNull", filtro.FEnvioNull);
                    command.Parameters.AddWithValue("@Empleado", (object?)filtro.Empleado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CompañiaT", (object?)filtro.CompañiaT ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DirigidoA", (object?)filtro.DirigidoA ?? DBNull.Value);
                }
            }
            return ventas;
        }
    }
}
