using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class ClienteService
    {
        private readonly ComboDataHelper comboDataHelper;
        private readonly ClienteDAL clienteDAL;
        public ClienteService(string connectionString)
        {
            comboDataHelper = new ComboDataHelper(connectionString);
            clienteDAL = new ClienteDAL(connectionString);
        }
        public List<ComboItemDto> ObtenerClientesPaisesCbo()
        {
            return comboDataHelper.LlenarCbo("SpClienteObtenerPaisesCbo");
        }
    }
}
