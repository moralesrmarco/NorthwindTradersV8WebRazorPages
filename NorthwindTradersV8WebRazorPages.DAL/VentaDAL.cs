using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class VentaDAL
    {
        private readonly string connectionString;
        public VentaDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            orderId = 0;
            rowVersion = null;
            int filasAfectadas = 0;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // 1) Insertar registro padre (SP)
                            using (var cmd = new SqlCommand("SpVentaInsertar", cn, tx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Parámetro de retorno
                                var returnParam = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                returnParam.Direction = ParameterDirection.ReturnValue;

                                // Parámetros de salida
                                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Direction = ParameterDirection.Output;
                                // RowVersion
                                cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
                                // Parámetros de entrada
                                cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrWhiteSpace(venta.Cliente.CustomerID) ? (object)DBNull.Value : venta.Cliente.CustomerID);
                                cmd.Parameters.AddWithValue("@EmployeeID", venta.Empleado.EmployeeID);
                                cmd.Parameters.AddWithValue("@OrderDate", venta.OrderDate.HasValue ? (object)venta.OrderDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@RequiredDate", venta.RequiredDate.HasValue ? (object)venta.RequiredDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShippedDate", venta.ShippedDate.HasValue ? (object)venta.ShippedDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShipVia", ((object)venta.Transportista.ShipperID == null || venta.Transportista.ShipperID.Equals(0)) ? DBNull.Value : (object)venta.Transportista.ShipperID);
                                cmd.Parameters.AddWithValue("@ShipName", string.IsNullOrWhiteSpace(venta.ShipName) ? (object)DBNull.Value : venta.ShipName);
                                cmd.Parameters.AddWithValue("@ShipAddress", string.IsNullOrWhiteSpace(venta.ShipAddress) ? (object)DBNull.Value : venta.ShipAddress);
                                cmd.Parameters.AddWithValue("@ShipCity", string.IsNullOrWhiteSpace(venta.ShipCity) ? (object)DBNull.Value : venta.ShipCity);
                                cmd.Parameters.AddWithValue("@ShipRegion", string.IsNullOrWhiteSpace(venta.ShipRegion) ? (object)DBNull.Value : venta.ShipRegion);
                                cmd.Parameters.AddWithValue("@ShipPostalCode", string.IsNullOrWhiteSpace(venta.ShipPostalCode) ? (object)DBNull.Value : venta.ShipPostalCode);
                                cmd.Parameters.AddWithValue("@ShipCountry", string.IsNullOrWhiteSpace(venta.ShipCountry) ? (object)DBNull.Value : venta.ShipCountry);
                                cmd.Parameters.AddWithValue("@Freight", (object)venta.Freight ?? DBNull.Value);

                                // Ejecutar y capturar código de retorno
                                cmd.ExecuteNonQuery();
                                // Capturar código de retorno
                                var returnValue = (int)returnParam.Value;
                                // filasAfectadas no depende de ExecuteNonQuery aquí,
                                // sino del código de retorno del SP
                                filasAfectadas = returnValue;
                                if (returnValue != 1)
                                {
                                    throw new InvalidOperationException("Error al insertar la venta. Código de error: " + returnValue);
                                }
                                // Capturar valores de salida
                                orderId = Convert.ToInt32(cmd.Parameters["@OrderID"].Value);
                                // Aquí obtienes el RowVersion como arreglo de bytes
                                rowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;

                                // Si quieres guardarlo en tu objeto Venta:
                                venta.RowVersion = rowVersion;
                            }
                            // 2) Preparar comandos reutilizables para cada detalle:
                            // 2.1) Preparar SELECT UnitsInStock FOR UPDATE
                            using (var cmdCheckStock = new SqlCommand("SpProductoObtenerInventarioPorIdConBloqueo", cn, tx)) // el bloqueo persiste para las demas operaciones dentro de la transacción
                            {
                                cmdCheckStock.CommandType = CommandType.StoredProcedure;
                                cmdCheckStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                // 2.2) Preparar UPDATE products
                                using (var cmdUpdateStock = new SqlCommand("SpProductoActualizarInventarioPorId", cn, tx))
                                {
                                    cmdUpdateStock.CommandType = CommandType.StoredProcedure;
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                    // 2.3) Preparar inserción de detalle (SP)
                                    using (var cmdInsertDetail = new SqlCommand("SpVentaDetalleInsertarSinActualizarInventario2", cn, tx))
                                    {
                                        cmdInsertDetail.CommandType = CommandType.StoredProcedure;
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.SmallInt));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Float));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@TasaIVA", SqlDbType.Float));

                                        // 3) Procesar cada detalle
                                        foreach (var d in venta.VentaDetalles)
                                        {
                                            // 3.1) Validar existencia y bloquear fila del producto
                                            cmdCheckStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var stockObj = cmdCheckStock.ExecuteScalar();
                                            if (stockObj == null || stockObj == DBNull.Value)
                                            {
                                                throw new InvalidOperationException($"Producto {d.Producto.ProductID} no existe.");
                                            }

                                            int currentStock = Convert.ToInt32(stockObj);

                                            // 3.2) Validar stock suficiente
                                            if (currentStock < d.Quantity)
                                            {
                                                throw new InvalidOperationException($"Inventario insuficiente para el producto {d.Producto.ProductID} {d.Producto.ProductName}. Disponible: {currentStock}, solicitado: {d.Quantity}.");
                                            }

                                            // 3.3) Actualizar stock
                                            cmdUpdateStock.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdUpdateStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var rowsUpdated = cmdUpdateStock.ExecuteNonQuery();
                                            if (rowsUpdated == 0)
                                            {
                                                throw new InvalidOperationException($"No se pudo actualizar el inventario para el producto {d.Producto.ProductID}.");
                                            }

                                            // 3.4) Insertar detalle (SP)
                                            cmdInsertDetail.Parameters["@OrderID"].Value = orderId;
                                            cmdInsertDetail.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            cmdInsertDetail.Parameters["@UnitPrice"].Value = d.UnitPrice;
                                            cmdInsertDetail.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdInsertDetail.Parameters["@Discount"].Value = d.Discount;
                                            cmdInsertDetail.Parameters["@TasaIVA"].Value = d.TasaIVA;

                                            filasAfectadas += cmdInsertDetail.ExecuteNonQuery();
                                        } // foreach detalles
                                    } // cmdInsertDetail
                                } // cmdUpdateStock
                            } // cmdCheckStock
                            tx.Commit();
                        }
                        catch (Exception)
                        {
                            try { tx.Rollback(); } catch { }
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la venta: " + ex.Message);
            }
            return filasAfectadas;
        }
        public int Eliminar(VentaDto venta, out string productoExcede)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", venta.OrderID);
                    cmd.Parameters.AddWithValue("@RowVersion", venta.RowVersion ?? (object)DBNull.Value);
                    var paramProductoExcede = cmd.Parameters.Add("@ProductoExcede", SqlDbType.VarChar, 40);
                    paramProductoExcede.Direction = ParameterDirection.Output;
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    paramProductoExcede.Value = paramProductoExcede.Value == DBNull.Value ? string.Empty : paramProductoExcede.Value;
                    numRegs = (int)returnParameter.Value;
                    productoExcede = paramProductoExcede.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la venta " + ex.Message);
            }
            return numRegs;
        }
        public DataTable ObtenerVentasPaginadas(int pageIndex, int pageSize, out int totalRegistros)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SpVentasObtenerPaginadas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageIndex", pageIndex);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    using (var dap = new SqlDataAdapter(command))
                    {
                        var ds = new DataSet();
                        dap.Fill(ds);
                        totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                        return ds.Tables[1];
                    }
                }
            }
        }
        public List<VentaDto> ObtenerVentasBuscadasPaginadas(VentasBuscarDto filtro, int pageIndex, int pageSize, out int totalRegistros)
        {
            var ventas = new List<VentaDto>();
            totalRegistros = 0;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("usp_ObtenerVentasBuscadasPaginadas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdIni", (object?)filtro.IdIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdFin", (object?)filtro.IdFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cliente", (object?)filtro.Cliente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVenta", filtro.FVenta);
                    command.Parameters.AddWithValue("@FVentaIni", (object?)filtro.FVentaIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVentaFin", (object?)filtro.FVentaFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FVentaNull", filtro.FVentaNull);
                    command.Parameters.AddWithValue("@FRequerido", filtro.FRequerido);
                    command.Parameters.AddWithValue("@FRequeridoIni", (object?)filtro.FRequeridoIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FRequeridoFin", (object?)filtro.FRequeridoFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FRequeridoNull", filtro.FRequeridoNull);
                    command.Parameters.AddWithValue("@FEnvio", filtro.FEnvio);
                    command.Parameters.AddWithValue("@FEnvioIni", (object?)filtro.FEnvioIni ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FEnvioFin", (object?)filtro.FEnvioFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FEnvioNull", filtro.FEnvioNull);
                    command.Parameters.AddWithValue("@Empleado", (object?)filtro.Empleado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CompañiaT", (object?)filtro.CompañiaT ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DirigidoA", (object?)filtro.DirigidoA ?? DBNull.Value);
                }
            }
            return ventas;
        }
        public VentaDto? ObtenerVentaPorId(int id)
        {
            VentaDto? venta = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", id);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rdr.Read())
                        {
                            venta = new VentaDto()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                CustomerCompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName")),
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                EmployeeName =
                                    (rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")))
                                    + ", " +
                                    (rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName"))),
                                ShipperCompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                    ? string.Empty
                                    : rdr["ShipperCompanyName"].ToString(),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion"))
                                    ? null
                                    : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public EnvioInformacionDto? ObtenerUltimaInformacionDeEnvio(string customerId)
        {
            EnvioInformacionDto? envioInformacion = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerUltimaInformacionDeEnvio", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    DataRow row = dt.Rows[0];
                    envioInformacion = new EnvioInformacionDto
                    {
                        ShipName = row["ShipName"] == DBNull.Value ? string.Empty : row["ShipName"].ToString(),
                        ShipAddress = row["ShipAddress"] == DBNull.Value ? string.Empty : row["ShipAddress"].ToString(),
                        ShipCity = row["ShipCity"] == DBNull.Value ? string.Empty : row["ShipCity"].ToString(),
                        ShipRegion = row["ShipRegion"] == DBNull.Value ? string.Empty : row["ShipRegion"].ToString(),
                        ShipPostalCode = row["ShipPostalCode"] == DBNull.Value ? string.Empty : row["ShipPostalCode"].ToString(),
                        ShipCountry = row["ShipCountry"] == DBNull.Value ? string.Empty : row["ShipCountry"].ToString(),
                        ShipVia = row["ShipVia"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(row["ShipVia"])
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la información de envío: " + ex.Message);
            }
            return envioInformacion;
        }
        public List<EnvioInformacionDto> ObtenerFormasEnvio(string customerID)
        {
            List<EnvioInformacionDto> lista = new();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SpVentaObtenerFormasEnvio", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(
                        "@CustomerID",
                        customerID
                    );
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new EnvioInformacionDto
                            {
                                CompanyName = dr["CompanyName"].ToString() ?? "",
                                ShipName = dr["ShipName"] == DBNull.Value
                                    ? null
                                    : dr["ShipName"].ToString(),
                                ShipAddress = dr["ShipAddress"] == DBNull.Value
                                    ? null
                                    : dr["ShipAddress"].ToString(),
                                ShipCity = dr["ShipCity"] == DBNull.Value
                                    ? null
                                    : dr["ShipCity"].ToString(),
                                ShipRegion = dr["ShipRegion"] == DBNull.Value
                                    ? null
                                    : dr["ShipRegion"].ToString(),
                                ShipPostalCode = dr["ShipPostalCode"] == DBNull.Value
                                    ? null
                                    : dr["ShipPostalCode"].ToString(),
                                ShipCountry = dr["ShipCountry"] == DBNull.Value
                                    ? null
                                    : dr["ShipCountry"].ToString(),
                                ShipVia = dr["ShipVia"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["ShipVia"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public Venta ObtenerVentaPorIdRpt(int orderId)
        {
            Venta venta = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rdr.HasRows)
                            throw new Exception($"SpVentaObtenerPorId no encontró la venta {orderId}");
                        if (rdr.Read())
                        {
                            venta = new Venta()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                Cliente = new Cliente
                                {
                                    CustomerID = rdr.IsDBNull(rdr.GetOrdinal("CustomerID")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerID")),
                                    CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName"))
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID")),
                                    LastName = rdr["LastName"].ToString(),
                                    FirstName = rdr["FirstName"].ToString()
                                },
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr.GetInt32(rdr.GetOrdinal("ShipVia")),
                                    CompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                      ? string.Empty
                                      : rdr["ShipperCompanyName"].ToString()
                                },
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public Venta? ObtenerVentaPorId2(int orderId)
        {
            Venta? venta = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rdr.Read())
                        {
                            venta = new Venta()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                Cliente = new Cliente
                                {
                                    CustomerID = rdr.IsDBNull(rdr.GetOrdinal("CustomerID")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerID")),
                                    CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName"))
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID")),
                                    LastName = rdr["LastName"].ToString(),
                                    FirstName = rdr["FirstName"].ToString()
                                },
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr.IsDBNull(rdr.GetOrdinal("ShipVia"))
                                        ? null
                                        : rdr.GetInt32(rdr.GetOrdinal("ShipVia")),
                                    CompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                      ? string.Empty
                                      : rdr["ShipperCompanyName"].ToString()
                                },
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public byte[] ActualizarEncabezado(
            int orderID,
            string customerID,
            int employeeID,
            DateTime? orderDate,
            DateTime? requiredDate,
            DateTime? shippedDate,
            byte[] rowVersion)
        {
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd =
                new("SpVentaActualizarEncabezado2", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderID;
            cmd.Parameters.Add("@CustomerID", SqlDbType.NChar, 5).Value =
                customerID ?? (object)DBNull.Value;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value =
                employeeID;
            cmd.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value =
                orderDate ?? (object)DBNull.Value;
            cmd.Parameters.Add("@RequiredDate", SqlDbType.DateTime).Value =
                requiredDate ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShippedDate", SqlDbType.DateTime).Value =
                shippedDate ?? (object)DBNull.Value;
            var parametroRowVersion = cmd.Parameters.Add(
                "@RowVersion",
                SqlDbType.Timestamp);
            parametroRowVersion.Value = rowVersion;
            parametroRowVersion.Direction = ParameterDirection.InputOutput;
            cn.Open();
            return (byte[])cmd.ExecuteScalar();
        }
        public (int Codigo, byte[]? RowVersion) ActualizarEncabezado2(
            int orderID,
            string customerID,
            int employeeID,
            DateTime? orderDate,
            DateTime? requiredDate,
            DateTime? shippedDate,
            byte[] rowVersion)
        {
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd =
                new("SpVentaActualizarEncabezado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderID;
            cmd.Parameters.Add("@CustomerID", SqlDbType.NChar, 5).Value =
                customerID ?? (object)DBNull.Value;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value =
                employeeID;
            cmd.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value =
                orderDate ?? (object)DBNull.Value;
            cmd.Parameters.Add("@RequiredDate", SqlDbType.DateTime).Value =
                requiredDate ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShippedDate", SqlDbType.DateTime).Value =
                shippedDate ?? (object)DBNull.Value;
            var parametroRowVersion = cmd.Parameters.Add(
                "@RowVersion",
                SqlDbType.Binary, 8);
            parametroRowVersion.Value =
                rowVersion ?? (object)DBNull.Value;
            parametroRowVersion.Direction =
                ParameterDirection.InputOutput;
            // Parámetro para recibir el RETURN del SP
            var parametroRetorno = cmd.Parameters.Add(
                "@ReturnValue",
                SqlDbType.Int);
            parametroRetorno.Direction =
                ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            int codigo = (int)parametroRetorno.Value;
            byte[]? nuevaRowVersion =
                parametroRowVersion.Value == DBNull.Value
                    ? null
                    : (byte[])parametroRowVersion.Value;
            return (codigo, nuevaRowVersion);
        }
        public byte[] ActualizarEnvio(
            int orderID,
            string? shipName,
            string? shipAddress,
            string? shipCity,
            string? shipRegion,
            string? shipPostalCode,
            string? shipCountry,
            int? shipVia,
            decimal? freight,
            byte[] rowVersion)
        {
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd =
                new("SpVentaActualizarEnvio", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int)
                .Value = orderID;
            cmd.Parameters.Add("@ShipName", SqlDbType.NVarChar, 40)
                .Value = shipName ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipAddress", SqlDbType.NVarChar, 60)
                .Value = shipAddress ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipCity", SqlDbType.NVarChar, 15)
                .Value = shipCity ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipRegion", SqlDbType.NVarChar, 15)
                .Value = shipRegion ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipPostalCode", SqlDbType.NVarChar, 10)
                .Value = shipPostalCode ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipCountry", SqlDbType.NVarChar, 15)
                .Value = shipCountry ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipVia", SqlDbType.Int)
                .Value = shipVia ?? (object)DBNull.Value;
            cmd.Parameters.Add("@Freight", SqlDbType.Money)
                .Value = freight ?? (object)DBNull.Value;
            var parametroRowVersion =
                cmd.Parameters.Add("@RowVersion",
                SqlDbType.Timestamp);
            parametroRowVersion.Value = rowVersion;
            parametroRowVersion.Direction =
                ParameterDirection.InputOutput;
            cn.Open();
            return (byte[])cmd.ExecuteScalar();
        }
        public (int Codigo, byte[]? RowVersion) ActualizarEnvio2(
            int orderID,
            string? shipName,
            string? shipAddress,
            string? shipCity,
            string? shipRegion,
            string? shipPostalCode,
            string? shipCountry,
            int? shipVia,
            decimal? freight,
            byte[] rowVersion)
        {
            using SqlConnection cn = new(connectionString);
            using SqlCommand cmd =
                new("SpVentaActualizarEnvio2", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int)
                .Value = orderID;
            cmd.Parameters.Add("@ShipName", SqlDbType.NVarChar, 40)
                .Value = shipName ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipAddress", SqlDbType.NVarChar, 60)
                .Value = shipAddress ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipCity", SqlDbType.NVarChar, 15)
                .Value = shipCity ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipRegion", SqlDbType.NVarChar, 15)
                .Value = shipRegion ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipPostalCode", SqlDbType.NVarChar, 10)
                .Value = shipPostalCode ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipCountry", SqlDbType.NVarChar, 15)
                .Value = shipCountry ?? (object)DBNull.Value;
            cmd.Parameters.Add("@ShipVia", SqlDbType.Int)
                .Value = shipVia ?? (object)DBNull.Value;
            cmd.Parameters.Add("@Freight", SqlDbType.Money)
                .Value = freight ?? (object)DBNull.Value;
            var parametroRowVersion = cmd.Parameters.Add(
                "@RowVersion",
                SqlDbType.Binary, 8);
            parametroRowVersion.Value =
                rowVersion ?? (object)DBNull.Value;
            parametroRowVersion.Direction =
                ParameterDirection.InputOutput;
            // Parámetro para recibir el RETURN del SP
            var parametroRetorno = cmd.Parameters.Add(
                "@ReturnValue",
                SqlDbType.Int);
            parametroRetorno.Direction =
                ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            int codigo = (int)parametroRetorno.Value;
            byte[]? nuevaRowVersion =
                parametroRowVersion.Value == DBNull.Value
                    ? null
                    : (byte[])parametroRowVersion.Value;
            return (codigo, nuevaRowVersion);
        }
        public List<VentaDto> BuscarVentas(VentasBuscarDto filtro)
        {
            var ventas = new List<VentaDto>();
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SpVentaBuscar", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
                    command.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
                    command.Parameters.AddWithValue("@Cliente", (object?)filtro.Cliente ?? "");
                    command.Parameters.AddWithValue("@FVenta", filtro.FVenta);
                    command.Parameters.AddWithValue("@FVentaIni", (object?)filtro.FVentaIni ?? DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@FVentaFin",
                        filtro.FVentaFin.HasValue
                            ? filtro.FVentaFin.Value.Date.AddDays(1)
                            : DBNull.Value);

                    command.Parameters.AddWithValue("@FVentaNull", filtro.FVentaNull);
                    command.Parameters.AddWithValue("@FRequerido", filtro.FRequerido);
                    command.Parameters.AddWithValue("@FRequeridoIni", (object?)filtro.FRequeridoIni ?? DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@FRequeridoFin",
                        filtro.FRequeridoFin.HasValue
                            ? filtro.FRequeridoFin.Value.Date.AddDays(1)
                            : DBNull.Value);

                    command.Parameters.AddWithValue("@FRequeridoNull", filtro.FRequeridoNull);
                    command.Parameters.AddWithValue("@FEnvio", filtro.FEnvio);
                    command.Parameters.AddWithValue("@FEnvioIni", (object?)filtro.FEnvioIni ?? DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@FEnvioFin",
                        filtro.FEnvioFin.HasValue
                            ? filtro.FEnvioFin.Value.Date.AddDays(1)
                            : DBNull.Value);

                    command.Parameters.AddWithValue("@FEnvioNull", filtro.FEnvioNull);
                    command.Parameters.AddWithValue("@Empleado", (object?)filtro.Empleado ?? "");
                    command.Parameters.AddWithValue("@CompañiaT", (object?)filtro.CompañiaT ?? "");
                    command.Parameters.AddWithValue("@DirigidoA", (object?)filtro.DirigidoA ?? "");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ordOrderDate = reader.GetOrdinal("OrderDate");
                            int ordRequiredDate = reader.GetOrdinal("RequiredDate");
                            int ordShippedDate = reader.GetOrdinal("ShippedDate");
                            ventas.Add(new VentaDto
                            {
                                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                CustomerCompanyName =
                                    reader.IsDBNull(reader.GetOrdinal("CustomerCompanyName"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("CustomerCompanyName")),
                                CustomerContactName =
                                    reader.IsDBNull(reader.GetOrdinal("ContactName"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("ContactName")),
                                OrderDate =
                                    reader.IsDBNull(ordOrderDate)
                                        ? null
                                        : reader.GetDateTime(ordOrderDate),
                                RequiredDate =
                                    reader.IsDBNull(ordRequiredDate)
                                        ? null
                                        : reader.GetDateTime(ordRequiredDate),
                                ShippedDate =
                                    reader.IsDBNull(ordShippedDate)
                                        ? null
                                        : reader.GetDateTime(ordShippedDate)
                            });
                        }
                    }
                }
            }
            return ventas;
        }
    }
}