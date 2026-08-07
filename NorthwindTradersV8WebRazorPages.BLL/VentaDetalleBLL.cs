using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class VentaDetalleBLL
    {
        private readonly VentaDetalleDAL ventaDetalleDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public VentaDetalleBLL(string connectionString)
        {
            ventaDetalleDAL = new VentaDetalleDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora= 0;
        }
        public VentaDetalleBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            ventaDetalleDAL = new VentaDetalleDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return ventaDetalleDAL.ObtenerVentaDetallePorVentaId(orderId);
        }
        public void InsertarDetalle(VentaDetalle detalle)
        {
            ventaDetalleDAL.InsertarDetalle(detalle);
        }
        public List<VentaDetalle> ObtenerDetallesPorVentaId(int orderID)
        {
            return ventaDetalleDAL.ObtenerDetallesPorVentaId(orderID);
        }
    }
}
