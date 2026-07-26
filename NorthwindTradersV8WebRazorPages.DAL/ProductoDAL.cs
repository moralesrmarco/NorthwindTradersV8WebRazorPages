using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
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
        public DataTable BuscarProductos(ProductosBuscarDto filtro)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpProductoBuscarV4", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
            cmd.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
            cmd.Parameters.AddWithValue("@Producto", string.IsNullOrEmpty(filtro.Producto) ? DBNull.Value : filtro.Producto);
            cmd.Parameters.AddWithValue("@Categoria", filtro.Categoria.HasValue ? filtro.Categoria.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Proveedor", filtro.Proveedor.HasValue ? filtro.Proveedor.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OrdenadoPor", "PRODUCTID");
            cmd.Parameters.AddWithValue("@AscDesc", "ASC");
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        public List<ProductoDto> ObtenerProductosAlfabeticoRpt()
        {
            var productos = new List<ProductoDto>();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoBuscarV4", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdIni", 0);
                    cmd.Parameters.AddWithValue("@IdFin", 0);
                    cmd.Parameters.AddWithValue("@Producto","");
                    cmd.Parameters.AddWithValue("@Categoria", 0);
                    cmd.Parameters.AddWithValue("@Proveedor", 0);
                    cmd.Parameters.AddWithValue("@OrdenadoPor", "ProductName");
                    cmd.Parameters.AddWithValue("@AscDesc", "ASC");
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new ProductoDto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos para el reporte alfabético.", ex);
            }
            return productos;
        }
        public List<ProductoDto> ObtenerProductosRpt(ProductosBuscarDto criterios)
        {
            var productos = new List<ProductoDto>();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SpProductoBuscarV4";
                    cmd.Parameters.AddWithValue("@IdIni", criterios.IdIni ?? 0);
                    cmd.Parameters.AddWithValue("@IdFin", criterios.IdFin ?? 0);
                    cmd.Parameters.AddWithValue("@Producto", criterios.Producto ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Categoria", criterios.Categoria ?? 0);
                    cmd.Parameters.AddWithValue("@Proveedor", criterios.Proveedor ?? 0);
                    cmd.Parameters.AddWithValue("@OrdenadoPor", criterios.OrdenadoPor);
                    cmd.Parameters.AddWithValue("@AscDesc", criterios.AscDesc);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new ProductoDto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productos;
        }
        public decimal ObtenerPrecioPromedio()
        {
            decimal precioPromedio = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("Select Avg(UnitPrice) As PrecioPromedio from products", con))
                {
                    con.Open();
                    precioPromedio = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al calcular el precio promedio: " + ex.Message);
            }
            return precioPromedio;
        }
        public DataTable ObtenerProductosPorEncimaDelPrecioPromedio()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("Select * from VwProductosPorEncimaDelPrecioPromedio", con))
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los productos por encima del precio promedio: " + ex.Message);
            }
            return dt;
        }
        public ProductoCostoEInventarioDto? ObtenerProductoCostoEInventario(int productId)
        {
            ProductoCostoEInventarioDto? dtoProductoCostoEInventario = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductoObtenerCostoEInventario", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ProductID", productId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    DataRow dr = dt.Rows[0];
                    dtoProductoCostoEInventario = new ProductoCostoEInventarioDto
                    {
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["UnitPrice"]),
                        UnitsInStock = dr["UnitsInStock"] == DBNull.Value ? (short)0 : Convert.ToInt16(dr["UnitsInStock"])
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el costo e inventario del producto: " + ex.Message);
            }
            return dtoProductoCostoEInventario;
        }
    }
}