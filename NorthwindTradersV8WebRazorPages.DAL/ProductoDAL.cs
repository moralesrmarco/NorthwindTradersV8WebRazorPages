using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class ProductoDAL
    {
        private readonly string connectionString;
        public ProductoDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerProductos()
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT ProductID, ProductName, UnitPrice, UnitsInStock, Discontinued FROM Products order by productid desc", conn);
            using var adapter = new SqlDataAdapter(cmd);

            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public DataTable ObtenerProductosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpProductosObtenerPaginados", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var adapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);

            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = productos paginados
            return ds.Tables[1];
        }


        public int Insertar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", 0);
                    cmd.Parameters["@ProductID"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", string.IsNullOrEmpty(producto.QuantityPerUnit) ? DBNull.Value : producto.QuantityPerUnit);
                    cmd.Parameters.AddWithValue("@UnitPrice", producto.UnitPrice.HasValue ? producto.UnitPrice.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", producto.UnitsInStock.HasValue ? producto.UnitsInStock.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", producto.UnitsOnOrder.HasValue ? producto.UnitsOnOrder.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", producto.ReorderLevel.HasValue ? producto.ReorderLevel.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return numRegs;
        }

        public int Actualizar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", producto.ProductID);
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", string.IsNullOrEmpty(producto.QuantityPerUnit) ? DBNull.Value : producto.QuantityPerUnit);
                    cmd.Parameters.AddWithValue("@UnitPrice", producto.UnitPrice.HasValue ? producto.UnitPrice.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", producto.UnitsInStock.HasValue ? producto.UnitsInStock.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", producto.UnitsOnOrder.HasValue ? producto.UnitsOnOrder.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", producto.ReorderLevel.HasValue ? producto.ReorderLevel.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    cmd.Parameters.AddWithValue("@RowVersion", producto.RowVersion ?? (object)DBNull.Value);
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
        public int Eliminar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", producto.ProductID);
                    cmd.Parameters.AddWithValue("@RowVersion", producto.RowVersion ?? (object)DBNull.Value);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (SqlException)
            {
                throw; // Otros errores se relanzan
            }
            return numRegs;
        }

        public Producto? ObtenerProductoPorId(int id)
        {
            Producto? producto = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", id);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                RowVersion = reader["RowVersion"] != DBNull.Value ? (byte[])reader["RowVersion"] : null,

                                // Relación con Proveedor
                                Proveedor = new Proveedor
                                {
                                    SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                    CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                                },
                                // Relación con Categoria
                                Categoria = new Categoria
                                {
                                    CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                    CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return producto;
        }
    }
}
