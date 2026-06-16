using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class EmpleadoBLL
    {
        private readonly EmpleadoDAL empleadoDAL;
        private readonly bool _ejecutarTiempoDemora;
        private readonly int _tiempoDemora;

        public EmpleadoBLL(string connectionString)
        {
            empleadoDAL = new EmpleadoDAL(connectionString);
            _ejecutarTiempoDemora = false;
            _tiempoDemora = 0;
        }
        public EmpleadoBLL(string connectionString, bool ejecutarTiempoDemora, int tiempoDemora)
        {
            empleadoDAL = new EmpleadoDAL(connectionString);
            _ejecutarTiempoDemora = ejecutarTiempoDemora;
            _tiempoDemora = tiempoDemora;
        }

        public ResultadoOperacion Insertar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = empleadoDAL.Insertar(empleado);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = empleadoDAL.Actualizar(empleado);
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
        public ResultadoOperacion Eliminar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = empleadoDAL.Eliminar(empleado);
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
        public DataTable ObtenerEmpleadosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            return empleadoDAL.ObtenerEmpleadosPaginados(pageIndex, pageSize, out totalRegistros);
        }
        public byte[]? ObtenerEmpleadoFotoPorId(int id)
        {
            return empleadoDAL.ObtenerEmpleadoFotoPorId(id);
        }

        public Empleado? ObtenerEmpleadoPorId(int id)
        {
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return empleadoDAL.ObtenerEmpleadoPorId(id);
        }
        public EmpleadoRptDto? ObtenerEmpleadoPorIdRptDto(int id) =>
            empleadoDAL.ObtenerEmpleadoPorIdRptDto(id);

        public DataTable BuscarEmpleados(EmpleadosBuscarDto filtro)
        {
            if (filtro.Pais == "0")
            {
                filtro.Pais = "";
            }
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return empleadoDAL.BuscarEmpleados(filtro);
        }
        public List<Empleado> ObtenerTodosLosEmpleados()
        {
            var empleados = empleadoDAL.ObtenerTodosLosEmpleados();
            if (_ejecutarTiempoDemora)
                Thread.Sleep(_tiempoDemora);
            return empleados;
        }

    }
}
