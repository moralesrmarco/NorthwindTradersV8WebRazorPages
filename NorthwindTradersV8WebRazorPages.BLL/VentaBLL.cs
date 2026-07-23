using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
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

    }
}
