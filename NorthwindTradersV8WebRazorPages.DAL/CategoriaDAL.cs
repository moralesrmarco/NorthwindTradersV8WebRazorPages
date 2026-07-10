using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
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
        public int Insertar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaInsertar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", categoria.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Picture", categoria.Picture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", SqlDbType.Int).Direction = ParameterDirection.Output;
                    conn.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la categoria " + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaActualizar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoria.CategoryID);
                    cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", categoria.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Picture", categoria.Picture ?? (object)DBNull.Value);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = categoria.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la categoria " + ex.Message);
            }
            return numRegs;
        }
        public int Eliminar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaEliminar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoria.CategoryID);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = categoria.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la categoria " + ex.Message);
            }
            return numRegs;
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
        public byte[]? ObtenerCategoriaPicturePorId(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriasObtenerPicturePorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", id);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    byte[]? pictureBytes = null;
                    if (result == null || result == DBNull.Value)
                    {
                        // Cargar la imagen por defecto desde wwwroot/images
                        var defaultPath = Path.Combine("wwwroot", "images", "Categorias.png");
                        pictureBytes = File.ReadAllBytes(defaultPath);
                    }
                    else
                        pictureBytes = (byte[])result;
                    return PhotoHelper.StripOleHeader(pictureBytes, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la imagen de la categoria " + ex.Message);
            }
        }
        public Categoria? ObtenerCategoriaPorId(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaObtenerPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Categoria
                            {
                                CategoryID = dr["CategoryID"] != DBNull.Value ? Convert.ToInt32(dr["CategoryID"]) : 0,
                                CategoryName = dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null,
                                Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null,
                                Picture = ObtenerCategoriaPicturePorId(id),
                                RowVersion = dr["RowVersion"] != DBNull.Value ? (byte[])dr["RowVersion"] : null
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la categoria por ID " + ex.Message);
            }
        }
        public DataTable BuscarCategorias(CategoriasBuscarDto filtro)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaBuscar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
                    cmd.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
                    cmd.Parameters.AddWithValue("@CategoryName", filtro.CategoryName ?? "");
                    using (var dap = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        dap.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar las categorias " + ex.Message);
            }
        }
        public List<Categoria> ObtenerCategoriasRpt()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpCategoriaObtener", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@top100", 1);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var categorias = new List<Categoria>();
                        while (dr.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                CategoryID = dr["CategoryID"] != DBNull.Value ? Convert.ToInt32(dr["CategoryID"]) : 0,
                                CategoryName = dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null,
                                Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null,
                                Picture = ObtenerCategoriaPicturePorId(Convert.ToInt32(dr["CategoryID"]))
                            });
                        }
                        return categorias;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorias para el reporte " + ex.Message);
            }
        }
        public List<CategoriasConProductosDto> ObtenerCategoriasConProductosRpt()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("Select * from VwCategoriasConProductos Order by CategoryName, ProductName", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var categoriasConProductos = new List<CategoriasConProductosDto>();
                        while (dr.Read())
                        {
                            categoriasConProductos.Add(new CategoriasConProductosDto
                            {
                                CategoryName = dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null,
                                CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null,
                                ProductID = dr["ProductID"] != DBNull.Value ? Convert.ToInt32(dr["ProductID"]) : null,
                                ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString() : null,
                                QuantityPerUnit = dr["QuantityPerUnit"] != DBNull.Value ? dr["QuantityPerUnit"].ToString() : null,
                                UnitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : null,
                                UnitsInStock = dr["UnitsInStock"] != DBNull.Value ? Convert.ToInt16(dr["UnitsInStock"]) : null,
                                UnitsOnOrder = dr["UnitsOnOrder"] != DBNull.Value ? Convert.ToInt16(dr["UnitsOnOrder"]) : null,
                                ReorderLevel = dr["ReorderLevel"] != DBNull.Value ? Convert.ToInt16(dr["ReorderLevel"]) : null,
                                Discontinued = dr["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dr["Discontinued"]) : false
                            });
                        }
                        return categoriasConProductos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorias con productos para el reporte " + ex.Message);
            }
        }
        public List<Producto> ObtenerProductosPorCategoriaId(int categoriaId)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpProductosObtenerPorCategoriaId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoriaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var productos = new List<Producto>();
                        while (dr.Read())
                        {
                            productos.Add(new Producto
                            {
                                ProductID = dr["ProductID"] != DBNull.Value ? Convert.ToInt32(dr["ProductID"]) : 0,
                                ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString() : null,
                                QuantityPerUnit = dr["QuantityPerUnit"] != DBNull.Value ? dr["QuantityPerUnit"].ToString() : null,
                                UnitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : null,
                                UnitsInStock = dr["UnitsInStock"] != DBNull.Value ? Convert.ToInt16(dr["UnitsInStock"]) : null,
                                UnitsOnOrder = dr["UnitsOnOrder"] != DBNull.Value ? Convert.ToInt16(dr["UnitsOnOrder"]) : null,
                                ReorderLevel = dr["ReorderLevel"] != DBNull.Value ? Convert.ToInt16(dr["ReorderLevel"]) : null,
                                Discontinued = dr["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dr["Discontinued"]) : false,
                                Proveedor = new Proveedor
                                {
                                    CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null
                                }
                            });
                        }
                        return productos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos por categoria " + ex.Message);
            }
        }
    }
}
