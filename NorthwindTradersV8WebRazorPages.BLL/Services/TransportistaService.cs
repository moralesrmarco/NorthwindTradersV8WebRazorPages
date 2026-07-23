using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class TransportistaService
    {
        private readonly ComboDataHelper comboDataHelper;
        public TransportistaService(string connectionString)
        {
            comboDataHelper = new ComboDataHelper(connectionString);
        }
        public List<ComboItemDto> ObtenerTransportistasCbo()
        {
            return comboDataHelper.LlenarCbo("SpTransportistaObtenerCbo");
        }
    }
}
