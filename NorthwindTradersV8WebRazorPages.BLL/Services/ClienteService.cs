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
        public List<KeyValuePair<string, string>> ObtenerCiudadesPaisesVwCliProvCbo()
        {
            var ciudadesPaises = clienteDAL.ObtenerCiudadesPaisesVwCliProvCbo();
            var ciudadesPaisesKvp = new List<KeyValuePair<string, string>>();
            ciudadesPaisesKvp.Add(new KeyValuePair<string, string>("»--- Seleccione ---«", ""));
            // Insertar opción "Todas las ciudades"
            ciudadesPaisesKvp.Add(new KeyValuePair<string, string>("»--- Todas las ciudades ---«", "00000"));
            // Agregar el resto de ciudades desde la DAL
            foreach (var item in ciudadesPaises)
            {
                ciudadesPaisesKvp.Add(new KeyValuePair<string, string>(item.CiudadPais, item.CiudadPais));
            }
            return ciudadesPaisesKvp;
        }
    }
}
