using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class ClienteDAL
    {
        private readonly string connectionString;
        public ClienteDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerClientesPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("SpClientesObtenerPaginados", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageIndex", pageIndex);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var adapter = new SqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = clientes paginados
            return ds.Tables[1];
        }
    }
}
