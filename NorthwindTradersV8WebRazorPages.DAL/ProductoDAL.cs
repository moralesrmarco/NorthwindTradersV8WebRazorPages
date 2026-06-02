using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class ProductoDAL
    {
        private readonly string _connectionString;
        public ProductoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DataTable ObtenerProductos()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT ProductID, ProductName, UnitPrice FROM Products", conn);
            using var adapter = new SqlDataAdapter(cmd);

            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public int Insertar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("SpProductoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", 0);
                    cmd.Parameters["@ProductID"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", (object)producto.QuantityPerUnit ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", (object)producto.UnitPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", (object)producto.UnitsInStock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", (object)producto.UnitsOnOrder ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", (object)producto.ReorderLevel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                    producto.ProductID = (int)cmd.Parameters["@ProductID"].Value;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return numRegs;
        }

        public void Actualizar(int id, string nombre, decimal precio)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Products SET ProductName=@nombre, UnitPrice=@precio WHERE ProductID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@precio", precio);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int Eliminar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(_connectionString))
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
                using (var con = new SqlConnection(_connectionString))
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
