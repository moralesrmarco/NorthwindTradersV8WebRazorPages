using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;

namespace NorthwindTradersV8WebRazorPages.BLL.Services
{
    public class VentaService
    {
        private readonly VentaDAL ventaDAL;
        public VentaService(string connectionString)
        {
            ventaDAL = new VentaDAL(connectionString);
        }
        public EnvioInformacionDto? ObtenerUltimaInformacionDeEnvio(string customerId)
        {
            return ventaDAL.ObtenerUltimaInformacionDeEnvio(customerId);
        }
        public List<EnvioInformacionDto> ObtenerFormasEnvio(string customerId)
        {
            return ventaDAL.ObtenerFormasEnvio(customerId);
        }
    }
}