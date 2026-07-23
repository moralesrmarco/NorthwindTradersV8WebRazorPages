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
        public List<KeyValuePair<string, string>> ObtenerPaisesVwCliProvCbo()
        {
            var paises = clienteDAL.ObtenerPaisesVwCliProvCbo();
            var paisesKvp = new List<KeyValuePair<string, string>>();
            paisesKvp.Add(new KeyValuePair<string, string>("»--- Seleccione ---«", ""));
            paisesKvp.Add(new KeyValuePair<string, string>("»--- Todos los paises ---«", "00000"));
            foreach (var item in paises)
            {
                paisesKvp.Add(new KeyValuePair<string, string>(item.Pais ?? string.Empty, item.Pais ?? string.Empty));
            }
            return paisesKvp;
        }
        public List<ComboItemDto> ObtenerClientesCbo()
        {
            return comboDataHelper.LlenarCbo("SpClienteObtenerCbo");
        }
    }
}
