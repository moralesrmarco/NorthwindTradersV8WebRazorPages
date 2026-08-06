using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class VentaBLL
    {
        private readonly VentaDAL ventaDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public VentaBLL(string connectionString)
        {
            ventaDAL = new VentaDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public VentaBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            ventaDAL = new VentaDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public ResultadoOperacion InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = ventaDAL.InsertarVentaCompleta(venta, out orderId, out rowVersion);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Eliminar(VentaDto venta)
        {
            var resultado = new ResultadoOperacion();
            string productoExcede = "";
            int numRegs = ventaDAL.Eliminar(venta, out productoExcede);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -7)
                resultado.Mensaje = $"No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades)";
            else if (numRegs == -8)
                resultado.Mensaje = $"No fue eliminada de la base de datos, el nuevo inventario del producto {productoExcede}, sería invalido (negativo)";
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public DataTable ObtenerVentasPaginadas(int pageIndex, int pageSize, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return ventaDAL.ObtenerVentasPaginadas(pageIndex, pageSize, out totalRegistros);
        }
        public List<VentaDto> ObtenerVentasBuscadasPaginadas(VentasBuscarDto filtro, int pageIndex, int pageSize, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return ventaDAL.ObtenerVentasBuscadasPaginadas(filtro, pageIndex, pageSize, out totalRegistros);
        }
        public VentaDto? ObtenerVentaPorId(int id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return ventaDAL.ObtenerVentaPorId(id);
        }
        public DataTable ObtenerVentaPorIdDt(int orderId)
        {
            Venta venta = ventaDAL.ObtenerVentaPorIdRpt(orderId);
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Vendedor", typeof(string));
            dt.Columns.Add("FechaDePedido", typeof(DateTime));
            dt.Columns.Add("FechaRequerido", typeof(DateTime));
            dt.Columns.Add("FechaDeEnvio", typeof(DateTime));
            dt.Columns.Add("CompaniaTransportista", typeof(string));
            dt.Columns.Add("DirigidoA", typeof(string));
            dt.Columns.Add("Domicilio", typeof(string));
            dt.Columns.Add("Ciudad", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("CodigoPostal", typeof(string));
            dt.Columns.Add("Pais", typeof(string));
            dt.Columns.Add("Flete", typeof(decimal));
            DataRow dr = dt.NewRow();
            dr["Id"] = venta.OrderID;
            dr["Cliente"] = venta.Cliente.CompanyName;
            dr["Vendedor"] = venta.Empleado.NameByLastName;
            dr["FechaDePedido"] = venta.OrderDate ?? (object)DBNull.Value;
            dr["FechaRequerido"] = venta.RequiredDate ?? (object)DBNull.Value;
            dr["FechaDeEnvio"] = venta.ShippedDate ?? (object)DBNull.Value;
            dr["CompaniaTransportista"] = venta.Transportista.CompanyName;
            dr["DirigidoA"] = venta.ShipName;
            dr["Domicilio"] = venta.ShipAddress;
            dr["Ciudad"] = venta.ShipCity;
            dr["Region"] = venta.ShipRegion;
            dr["CodigoPostal"] = venta.ShipPostalCode;
            dr["Pais"] = venta.ShipCountry;
            dr["Flete"] = venta.Freight;
            dt.Rows.Add(dr);
            return dt;
        }
        public Venta? ObtenerVentaPorId2(int orderId)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return ventaDAL.ObtenerVentaPorId2(orderId);
        }
        public byte[] ActualizarEncabezado(ActualizarEncabezadoRequest request)
        {
            var rowVersion = Convert.FromBase64String(request.RowVersion);
            DateTime? orderDate = CombinarFechaHora(
                request.OrderDate,
                request.OrderTime);
            DateTime? requiredDate = CombinarFechaHora(
                request.RequiredDate,
                request.RequiredTime);
            DateTime? shippedDate = CombinarFechaHora(
                request.ShippedDate,
                request.ShippedTime);
            return ventaDAL.ActualizarEncabezado(
                request.OrderID,
                request.CustomerID,
                request.EmployeeID,
                orderDate,
                requiredDate,
                shippedDate,
                rowVersion);
        }
        private DateTime? CombinarFechaHora(
            DateTime? fecha,
            string? hora)
        {
            if (!fecha.HasValue)
                return null;
            if (TimeSpan.TryParse(hora, out TimeSpan tiempo))
            {
                return fecha.Value.Date.Add(tiempo);
            }
            return fecha.Value.Date;
        }
        public byte[] ActualizarEnvio(
            ActualizarEnvioRequest request)
        {
            var rowVersion =
                Convert.FromBase64String(request.RowVersion);
            return ventaDAL.ActualizarEnvio(
                request.OrderID,
                request.ShipName,
                request.ShipAddress,
                request.ShipCity,
                request.ShipRegion,
                request.ShipPostalCode,
                request.ShipCountry,
                request.ShipVia,
                request.Freight,
                rowVersion);
        }
    }
}
