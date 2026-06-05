using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class EmpleadoDAL
    {
        private readonly string connectionString;
        public EmpleadoDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerEmpleadosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpEmpleadosObtenerPaginados", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var adapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);

            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = empleados paginados
            return ds.Tables[1];
        }
        public byte[]? ObtenerEmpleadoFotoPorId(int employeeId)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpEmpleadoObtenerFotoPorId", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            connection.Open();
            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                return null;
            var fotoBytes = (byte[])result;
            return PhotoHelper.StripOleHeader(fotoBytes, employeeId);
        }
    }
}
