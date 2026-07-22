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

    }
}
