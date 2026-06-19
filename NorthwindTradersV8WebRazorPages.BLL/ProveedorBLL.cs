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
        public ResultadoOperacion Actualizar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = proveedorDAL.Actualizar(proveedor);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfmfe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfmfm;
            else
                resultado.Mensaje = StringsCommons.Nfmmd;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Eliminar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = proveedorDAL.Eliminar(proveedor);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -3)
                resultado.Mensaje = StringsCommons.Nferr;
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public Proveedor? ObtenerProveedorPorId(string id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return proveedorDAL.ObtenerProveedorPorId(id);
        }
        public DataTable ObtenerProveedoresPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return proveedorDAL.ObtenerProveedoresPaginados(pageIndex, rowsPerPage, out totalRegistros);
        }
    }
}
