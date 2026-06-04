using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ProductoBLL
    {
        private readonly ProductoDAL productoDAL;

        public ProductoBLL(string connectionString)
        {
            productoDAL = new ProductoDAL(connectionString);
        }
        public DataTable ObtenerProductosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            return productoDAL.ObtenerProductosPaginados(pageIndex, pageSize, out totalRegistros);
        }
        public ResultadoOperacion Insertar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
                
            int numRegs = productoDAL.Insertar(producto);
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            return resultado;
        }
        public ResultadoOperacion Actualizar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = productoDAL.Actualizar(producto);
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfmfe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfmfm;
            else
                resultado.Mensaje = StringsCommons.Nfmmd;
            return resultado;
        }
        public ResultadoOperacion Eliminar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = productoDAL.Eliminar(producto);
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
            return resultado;
        }
        public Producto? ObtenerProductoPorId(int id) => productoDAL.ObtenerProductoPorId(id);
    }
}
