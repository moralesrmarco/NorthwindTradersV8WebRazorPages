using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class ProductoService
    {
        private readonly ComboDataHelper comboDataHelper;
        private readonly ProductoDAL productoDAL;
        public ProductoService(string connectionString)
        {
            comboDataHelper = new ComboDataHelper(connectionString);
            productoDAL = new ProductoDAL(connectionString);
        }
        public List<ComboItemDto> ObtenerProductosPorCategoriaCbo(int categoriaId)
        {
            return comboDataHelper.LlenarCbo(
                "SpProductosObtenerPorCategoriaCbo",
                new SqlParameter("@Categoria", categoriaId)
            );
        }
        public ProductoCostoEInventarioDto? ObtenerProductoCostoEInventario(int productId)
        {
            return productoDAL.ObtenerProductoCostoEInventario(productId);
        }

    }
}
