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
        public DataTable ObtenerCategoriasPaginadas(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return categoriaDAL.ObtenerCategoriasPaginadas(pageIndex, rowsPerPage, out totalRegistros);
        }
    }
}
