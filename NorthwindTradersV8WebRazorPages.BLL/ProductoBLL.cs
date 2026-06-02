using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ProductoBLL
    {
        private readonly ProductoDAL _productoDAL;

        public ProductoBLL(string connectionString)
        {
            _productoDAL = new ProductoDAL(connectionString);
        }

        public DataTable ObtenerProductos() => _productoDAL.ObtenerProductos();

        public ResultadoOperacion Insertar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
                
            int numRegs = _productoDAL.Insertar(producto);
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            return resultado;
        }
        public void Actualizar(int id, string nombre, decimal precio) => _productoDAL.Actualizar(id, nombre, precio);

        public ResultadoOperacion Eliminar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _productoDAL.Eliminar(producto);
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

        public Producto? ObtenerProductoPorId(int id) => _productoDAL.ObtenerProductoPorId(id);
    }
}
