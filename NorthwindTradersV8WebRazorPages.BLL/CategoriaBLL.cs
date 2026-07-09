using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class CategoriaBLL
    {
        private readonly CategoriaDAL categoriaDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public CategoriaBLL(string connectionString)
        {
            categoriaDAL = new CategoriaDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public CategoriaBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            categoriaDAL = new CategoriaDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public ResultadoOperacion Insertar(Categoria categoria)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = categoriaDAL.Insertar(categoria);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Categoria categoria)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = categoriaDAL.Actualizar(categoria);
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
        public ResultadoOperacion Eliminar(Categoria categoria)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = categoriaDAL.Eliminar(categoria);
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
        public DataTable ObtenerCategoriasPaginadas(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerCategoriasPaginadas(pageIndex, rowsPerPage, out totalRegistros);
        }
        public byte[]? ObtenerCategoriaPicturePorId(int id)
        {
            return categoriaDAL.ObtenerCategoriaPicturePorId(id);
        }
        public Categoria? ObtenerCategoriaPorId(int id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerCategoriaPorId(id);
        }
        public DataTable BuscarCategorias(CategoriasBuscarDto filtro)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.BuscarCategorias(filtro);
        }
        public List<Categoria> ObtenerCategoriasRpt()
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerCategoriasRpt();
        }
        public List<CategoriasConProductosRptDto> ObtenerCategoriasConProductosRpt()
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerCategoriasConProductosRpt();
        }
        public List<Producto> ObtenerProductosPorCategoriaId(int categoriaId)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerProductosPorCategoriaId(categoriaId);
        }
    }
}
