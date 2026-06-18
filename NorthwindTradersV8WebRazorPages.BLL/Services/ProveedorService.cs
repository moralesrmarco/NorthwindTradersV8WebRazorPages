using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class ProveedorService
    {
        private readonly ProveedorDAL proveedorDAL;
        private readonly ComboDataHelper comboDataHelper;
        public ProveedorService(string connectionString)
        {
            proveedorDAL = new ProveedorDAL(connectionString);
            comboDataHelper = new ComboDataHelper(connectionString);
        }
        public List<ComboItemDto> ObtenerProveedoresPaisesCbo()
        {
            return comboDataHelper.LlenarCbo("SpProveedorObtenerPaisesCbo");
        }
    }
}
