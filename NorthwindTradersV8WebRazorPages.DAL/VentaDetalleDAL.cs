using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class VentaDetalleDAL
    {
        private readonly string connectionString;
        public VentaDetalleDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            List<VentaDetalle> ventaDetalles = new List<VentaDetalle>();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaDetalleObtenerPorVentaId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (rdr.Read())
                        {
                            var ventaDetalle = new VentaDetalle
                            {
                                Venta = new Venta
                                {
                                    OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID"))
                                },
                                Producto = new Producto
                                {
                                    ProductID = rdr.GetInt32(rdr.GetOrdinal("ProductID")),
                                    ProductName = rdr.IsDBNull(rdr.GetOrdinal("ProductName")) ? null : rdr.GetString(rdr.GetOrdinal("ProductName")),
                                    Categoria = rdr.IsDBNull(rdr.GetOrdinal("CategoryID"))
                                        ? null
                                        : new Categoria
                                        {
                                            CategoryID = rdr.GetInt32(rdr.GetOrdinal("CategoryID"))
                                        }
                                },
                                UnitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice")),
                                Quantity = rdr.GetInt16(rdr.GetOrdinal("Quantity")),
                                Discount = (decimal)rdr.GetFloat(rdr.GetOrdinal("Discount")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                            ventaDetalles.Add(ventaDetalle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la venta: " + ex.Message);
            }
            return ventaDetalles;
        }
        public void InsertarDetalle(VentaDetalle detalle)
        {
            try
            {
                using SqlConnection cn = new(connectionString);
                using SqlCommand cmd = new("SpVentaDetalleInsertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", detalle.Venta.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", detalle.Producto.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", detalle.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                cmd.Parameters.AddWithValue("@Discount", detalle.Discount);
                cmd.Parameters.AddWithValue("@TasaIVA", detalle.TasaIVA);
                SqlParameter pRowVersion = new("@VentaRowVersion", SqlDbType.Binary, 8)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = detalle.Venta.RowVersion ?? (object)DBNull.Value
                };
                cmd.Parameters.Add(pRowVersion);
                if (detalle.Venta.OrderID <= 0)
                    throw new ArgumentException("La venta no tiene un OrderID válido.");
                if (detalle.Producto.ProductID <= 0)
                    throw new ArgumentException("El producto no tiene un ProductID válido.");
                cn.Open();
                cmd.ExecuteNonQuery();
                detalle.Venta.RowVersion = (byte[])pRowVersion.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el detalle de la venta: " + ex.Message);
            }
        }
        public List<VentaDetalle> ObtenerDetallesPorVentaId(int orderID)
        {
            List<VentaDetalle> lista = new();
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd = new("SpVentaDetalleObtenerPorVentaId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderID);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new VentaDetalle
                {
                    Venta = new Venta
                    {
                        OrderID = (int)dr["OrderID"]
                    },
                    Producto = new Producto
                    {
                        ProductID = (int)dr["ProductID"],
                        ProductName = dr["ProductName"].ToString()
                    },
                    UnitPrice = (decimal)dr["UnitPrice"],
                    Quantity = (short)dr["Quantity"],
                    Discount = Convert.ToDecimal(dr["Discount"]),
                    TasaIVA = Convert.ToDecimal(dr["TasaIVA"])
                });
            }
            return lista;
        }
    }
}
