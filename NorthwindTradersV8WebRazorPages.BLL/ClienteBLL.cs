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
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;
        public ClienteBLL(string connectionString)
        {
            clienteDAL = new ClienteDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public ClienteBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            clienteDAL = new ClienteDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }
        public ResultadoOperacion Insertar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = clienteDAL.Insertar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = clienteDAL.Actualizar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfmfe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfmfm;
            else
                resultado.Mensaje = StringsCommons.Nfmmd;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }

        public ResultadoOperacion Eliminar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = clienteDAL.Eliminar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -3)
                resultado.Mensaje = StringsCommons.Nferr;
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public bool ExisteCliente(string customerID)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return clienteDAL.ExisteCliente(customerID);
        }
        public Cliente? ObtenerClientePorId(string id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return clienteDAL.ObtenerClientePorId(id);
        }
        public DataTable ObtenerClientesPaginados(int pageIndex, int rowsPerPage, out int totalRegistros)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return clienteDAL.ObtenerClientesPaginados(pageIndex, rowsPerPage, out totalRegistros);
        }
        public DataTable BuscarClientes(ClientesBuscarDto filtro)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return clienteDAL.BuscarClientes(filtro);
        }
    }
}
