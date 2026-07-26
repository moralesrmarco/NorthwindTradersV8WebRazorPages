using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class CategoriaService
    {
        private readonly ComboDataHelper comboDataHelper;
        private readonly CategoriaDAL categoriaDAL;
        public CategoriaService(string connectionString)
        {
            comboDataHelper = new ComboDataHelper(connectionString);
            categoriaDAL = new CategoriaDAL(connectionString);
        }
        public List<ComboItemDto> ObtenerCategoriasCbo()
        {
            return comboDataHelper.LlenarCbo("SpCategoriaObtenerCbo");
        }
    }
}
