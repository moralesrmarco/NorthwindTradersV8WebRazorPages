using Microsoft.Data.SqlClient;
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
        public int Insertar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpClienteInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }

            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new Exception(
                    $"El identificador de cliente <strong>'{cliente.CustomerID}'</strong> ya fue asignado previamente. <p>Proporcione un nuevo ID.</p>");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el cliente." + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpClienteActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = cliente.RowVersion ?? (object)DBNull.Value;
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
        public int Eliminar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpClienteEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerID);
                    cmd.Parameters.AddWithValue("@RowVersion", cliente.RowVersion);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = Convert.ToInt32(returnParameter.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el cliente." + ex.Message);
            }
            return numRegs;
        }

        public bool ExisteCliente(string customerID)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SpClienteExiste", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CustomerID", customerID);

                var pExiste = new SqlParameter("@Existe", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(pExiste);

                con.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToBoolean(pExiste.Value);
            }
        }
        public Cliente? ObtenerClientePorId(string id)
        {
            Cliente? cliente = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpClienteObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = MapearCliente(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el cliente por ID" + ex.Message);
            }
            return cliente;
        }

        public DataTable ObtenerClientesPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("SpClientesObtenerPaginados", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageIndex", pageIndex);
            command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);

            using var adapter = new SqlDataAdapter(command);
            var ds = new DataSet();
            adapter.Fill(ds);

            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = clientes paginados
            return ds.Tables[1];
        }
        private Cliente MapearCliente(SqlDataReader reader)
        {
            var cliente = new Cliente()
            {
                CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader["CustomerID"].ToString(),
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
            return cliente;
        }
        public DataTable BuscarClientes(ClientesBuscarDto filtro)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpClienteBuscar", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", filtro.CustomerID ?? "");
            cmd.Parameters.AddWithValue("@Compañia", filtro.CompanyName ?? "");
            cmd.Parameters.AddWithValue("@Contacto", filtro.ContactName ?? "");
            cmd.Parameters.AddWithValue("@Domicilio", filtro.Address ?? "");
            cmd.Parameters.AddWithValue("@Ciudad", filtro.City ?? "");
            cmd.Parameters.AddWithValue("@Region", filtro.Region ?? "");
            cmd.Parameters.AddWithValue("@CodigoP", filtro.PostalCode ?? "");
            cmd.Parameters.AddWithValue("@Pais", filtro.Country ?? "");
            cmd.Parameters.AddWithValue("@Telefono", filtro.Phone ?? "");
            cmd.Parameters.AddWithValue("@Fax", filtro.Fax ?? "");
            using var dap = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            dap.Fill(dt);
            return dt;
        }
    }
}
