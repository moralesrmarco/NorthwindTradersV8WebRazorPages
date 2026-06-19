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
        public int Actualizar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProveedorActualizar", con))
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
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = proveedor.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }
        public int Eliminar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProveedorEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@RowVersion", proveedor.RowVersion);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = Convert.ToInt32(returnParameter.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el proveedor." + ex.Message);
            }
            return numRegs;
        }
        public Proveedor? ObtenerProveedorPorId(string id)
        {
            Proveedor? proveedor = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProveedorObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            proveedor = MapearProveedor(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el proveedor por ID" + ex.Message);
            }
            return proveedor;
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
        private Proveedor MapearProveedor(SqlDataReader reader)
        {
            var proveedor = new Proveedor()
            {
                SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                ContactTitle = reader.IsDBNull(reader.GetOrdinal("ContactTitle")) ? null : reader["ContactTitle"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader["City"].ToString(),
                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader["Region"].ToString(),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader["PostalCode"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader["Fax"].ToString(),
                RowVersion = reader.IsDBNull(reader.GetOrdinal("RowVersion")) ? null : (byte[])reader["RowVersion"]
            };
            return proveedor;
        }
    }
}
