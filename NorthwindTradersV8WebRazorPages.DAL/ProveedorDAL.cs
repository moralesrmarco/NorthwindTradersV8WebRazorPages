using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;
using System.Reflection.PortableExecutable;

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
                throw new Exception("Error al obtener el proveedor por ID " + ex.Message);
            }
            return proveedor;
        }
        public DataTable ObtenerProveedoresPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand("SpProveedoresObtenerPaginados", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageIndex", pageIndex);
                command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                using var adapter = new SqlDataAdapter(command);
                var ds = new DataSet();
                adapter.Fill(ds);
                // Primer resultset = total de registros
                totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

                // Segundo resultset = proveedores paginados
                return ds.Tables[1];
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los proveedores " + ex.Message);
            }
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
        public DataTable BuscarProveedores(ProveedoresBuscarDto filtro)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand("SpProveedorBuscar", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
                cmd.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
                cmd.Parameters.AddWithValue("@CompanyName", filtro.CompanyName ?? "");
                cmd.Parameters.AddWithValue("@ContactName", filtro.ContactName ?? "");
                cmd.Parameters.AddWithValue("@Address", filtro.Address ?? "");
                cmd.Parameters.AddWithValue("@City", filtro.City ?? "");
                cmd.Parameters.AddWithValue("@Region", filtro.Region ?? "");
                cmd.Parameters.AddWithValue("@PostalCode", filtro.PostalCode ?? "");
                cmd.Parameters.AddWithValue("@Country", filtro.Country ?? "");
                cmd.Parameters.AddWithValue("@Phone", filtro.Phone ?? "");
                cmd.Parameters.AddWithValue("@Fax", filtro.Fax ?? "");
                using var dap = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                dap.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar los proveedores " + ex.Message);
            }
        }
        public List<Proveedor> ObtenerProveedoresRpt()
        {
            List<Proveedor> proveedores = new List<Proveedor>();
            try
            {
                using var conn = new SqlConnection(connectionString);
                using var cmd = new SqlCommand("SpProveedorObtener", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@top100", true);
                conn.Open();
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var proveedor = new Proveedor()
                    {
                        SupplierID = rdr.IsDBNull(rdr.GetOrdinal("SupplierID")) ? 0 : Convert.ToInt32(rdr["SupplierID"]),
                        CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CompanyName")) ? null : rdr["CompanyName"].ToString(),
                        ContactName = rdr.IsDBNull(rdr.GetOrdinal("ContactName")) ? null : rdr["ContactName"].ToString(),
                        ContactTitle = rdr.IsDBNull(rdr.GetOrdinal("ContactTitle")) ? null : rdr["ContactTitle"].ToString(),
                        Address = rdr.IsDBNull(rdr.GetOrdinal("Address")) ? null : rdr["Address"].ToString(),
                        City = rdr.IsDBNull(rdr.GetOrdinal("City")) ? null : rdr["City"].ToString(),
                        Region = rdr.IsDBNull(rdr.GetOrdinal("Region")) ? null : rdr["Region"].ToString(),
                        PostalCode = rdr.IsDBNull(rdr.GetOrdinal("PostalCode")) ? null : rdr["PostalCode"].ToString(),
                        Country = rdr.IsDBNull(rdr.GetOrdinal("Country")) ? null : rdr["Country"].ToString(),
                        Phone = rdr.IsDBNull(rdr.GetOrdinal("Phone")) ? null : rdr["Phone"].ToString(),
                        Fax = rdr.IsDBNull(rdr.GetOrdinal("Fax")) ? null : rdr["Fax"].ToString()
                    };
                    proveedores.Add(proveedor);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los proveedores " + ex.Message);
            }
            return proveedores;
        }
        public List<ProductosPorProveedorDto> ObtenerProductosPorProveedorRpt()
        {
            var productosPorProveedor = new List<ProductosPorProveedorDto>();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductosPorProveedorObtener", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productoPorProveedor = new ProductosPorProveedorDto()
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : (int?)null,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : "Sin producto",
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : string.Empty,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : string.Empty,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : (decimal?)null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : (short?)null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : (short?)null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : (short?)null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Sin categoría"
                            };
                            productosPorProveedor.Add(productoPorProveedor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos por proveedor " + ex.Message);
            }
            return productosPorProveedor;
        }
        public List<ProductosPorProveedorConDetProvDto> ObtenerProductosPorProveedorConDetalleDelProveedorRpt()
        {
            var productosPorProveedor = new List<ProductosPorProveedorConDetProvDto>();
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductosPorProveedorConDetProvObtener", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productoPorProveedor = new ProductosPorProveedorConDetProvDto
                            {
                                // Suppliers
                                SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : string.Empty,
                                ContactName = reader["ContactName"] != DBNull.Value ? reader["ContactName"].ToString() : string.Empty,
                                ContactTitle = reader["ContactTitle"] != DBNull.Value ? reader["ContactTitle"].ToString() : string.Empty,
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty,
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                                Region = reader["Region"] != DBNull.Value ? reader["Region"].ToString() : string.Empty,
                                PostalCode = reader["PostalCode"] != DBNull.Value ? reader["PostalCode"].ToString() : string.Empty,
                                Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
                                Fax = reader["Fax"] != DBNull.Value ? reader["Fax"].ToString() : string.Empty,

                                // Products
                                ProductID = reader["ProductID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ProductID"]) : null,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : "Sin producto",
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : string.Empty,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),

                                // Categories
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "Sin categoría"
                            };
                            productosPorProveedor.Add(productoPorProveedor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos por proveedor " + ex.Message);
            }
            return productosPorProveedor;
        }
    }
}
