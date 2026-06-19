using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class ProveedorDAL
    {
        private readonly string connectionString;
        public ProveedorDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int Insertar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProveedorInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierId", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@CompanyName", proveedor.CompanyName);
                    cmd.Parameters.AddWithValue("@ContactName", proveedor.ContactName);
                    cmd.Parameters.AddWithValue("@ContactTitle", proveedor.ContactTitle);
                    cmd.Parameters.AddWithValue("@Address", proveedor.Address);
                    cmd.Parameters.AddWithValue("@City", proveedor.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(proveedor.Region) ? (object)DBNull.Value : proveedor.Region);
                    cmd.Parameters.AddWithValue("@PostalCode", string.IsNullOrWhiteSpace(proveedor.PostalCode) ? (object)DBNull.Value : proveedor.PostalCode);
                    cmd.Parameters.AddWithValue("@Country", proveedor.Country);
                    cmd.Parameters.AddWithValue("@Phone", proveedor.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(proveedor.Fax) ? (object)DBNull.Value : proveedor.Fax);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el proveedor." + ex.Message);
            }
            return numRegs;
        }
        public DataTable ObtenerProveedoresPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("SpProveedoresObtenerPaginados", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageIndex", pageIndex);
            command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
            using var adapter = new SqlDataAdapter(command);
            var ds= new DataSet();
            adapter.Fill(ds);
            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = proveedores paginados
            return ds.Tables[1];
        }
    }
}
