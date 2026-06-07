using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class EmpleadoService
    {
        private readonly ComboDataHelper comboDataHelper;
        private readonly EmpleadoDAL empleadoDAL;
        public EmpleadoService(string connectionString)
        {
            comboDataHelper = new ComboDataHelper(connectionString);
            empleadoDAL = new EmpleadoDAL(connectionString);
        }
        public List<ComboItemDto> ObtenerEmpleadosPaisesCbo()
        {
            return comboDataHelper.LlenarCbo("SpEmpleadoObtenerPaisesCbo");
        }
        public List<ComboItemDto> ObtenerEmpleadoEmpleadosCbo()
        {
            return comboDataHelper.LlenarCbo("SpEmpleadoObtenerEmpleadosCbo");
        }
    }
}
