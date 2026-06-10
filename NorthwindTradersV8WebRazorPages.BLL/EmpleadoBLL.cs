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

        public EmpleadoBLL(string connectionString)
        {
            empleadoDAL = new EmpleadoDAL(connectionString);
        }
        public ResultadoOperacion Insertar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = empleadoDAL.Insertar(empleado);
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
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
            return empleadoDAL.BuscarEmpleados(filtro);
        }
        public List<Empleado> ObtenerTodosLosEmpleados()
        {
            var empleados = empleadoDAL.ObtenerTodosLosEmpleados();
            return empleados;
        }

    }
}
