using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
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
    }
}
