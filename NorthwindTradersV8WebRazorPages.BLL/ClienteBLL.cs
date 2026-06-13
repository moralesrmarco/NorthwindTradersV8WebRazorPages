using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class ClienteBLL
    {
        private readonly ClienteDAL clienteDAL;
        public ClienteBLL(string connectionString)
        {
            clienteDAL = new ClienteDAL(connectionString);
        }
        public ResultadoOperacion Insertar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = clienteDAL.Insertar(cliente);
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
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
            return resultado;
        }
        public bool ExisteCliente(string customerID) => clienteDAL.ExisteCliente(customerID);
        public Cliente? ObtenerClientePorId(string id) => clienteDAL.ObtenerClientePorId(id);
        public DataTable ObtenerClientesPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            return clienteDAL.ObtenerClientesPaginados(pageIndex, pageSize, out totalRegistros);
        }
    }
}
