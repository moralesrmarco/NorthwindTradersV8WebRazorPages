using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ProveedorBLL
    {
        private readonly ProveedorDAL proveedorDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public ProveedorBLL(string connectionString) 
        { 
            proveedorDAL = new ProveedorDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public ProveedorBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            proveedorDAL = new ProveedorDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public ResultadoOperacion Insertar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = proveedorDAL.Insertar(proveedor);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }

        public DataTable ObtenerProveedoresPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return proveedorDAL.ObtenerProveedoresPaginados(pageIndex, rowsPerPage, out totalRegistros);
        }
    }
}
