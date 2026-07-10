using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ProductoBLL
    {
        private readonly ProductoDAL productoDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public ProductoBLL(string connectionString)
        {
            productoDAL = new ProductoDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public ProductoBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            productoDAL = new ProductoDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public DataTable ObtenerProductosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return productoDAL.ObtenerProductosPaginados(pageIndex, pageSize, out totalRegistros);
        }
        public ResultadoOperacion Insertar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = productoDAL.Insertar(producto);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = productoDAL.Actualizar(producto);
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
        public ResultadoOperacion Eliminar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = productoDAL.Eliminar(producto);
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
        public Producto? ObtenerProductoPorId(int id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return productoDAL.ObtenerProductoPorId(id);
        }
    }
}
