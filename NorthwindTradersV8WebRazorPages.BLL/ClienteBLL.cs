using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ClienteBLL
    {
        private readonly ClienteDAL clienteDAL;
        public ClienteBLL(string connectionString)
        {
            clienteDAL = new ClienteDAL(connectionString);
        }
        public DataTable ObtenerClientesPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            return clienteDAL.ObtenerClientesPaginados(pageIndex, pageSize, out totalRegistros);
        }
    }
}
