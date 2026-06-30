using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class CategoriaDAL
    {
        private readonly string connectionString;
        public CategoriaDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerCategoriasPaginadas(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriasPaginadas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                    using (var dap = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        dap.Fill(ds);
                        totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                        return ds.Tables[1];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorias " + ex.Message);
            }
        }
    }
}
