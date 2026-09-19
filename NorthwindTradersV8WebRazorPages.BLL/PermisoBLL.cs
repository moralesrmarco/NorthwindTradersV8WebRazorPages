using NorthwindTradersV8WebRazorPages.DAL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class PermisoBLL
    {
        private readonly PermisoDAL permisoDAL;
        public PermisoBLL(string connectionString)
        {
            this.permisoDAL = new PermisoDAL(connectionString);
        }
        public HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            return permisoDAL.ObtenerPermisosPorUsuarioId(idUsuario);
        }
        public void InsertarPermiso(int idUsuario, int permisoId)
        {
            permisoDAL.InsertarPermiso(idUsuario, permisoId);
        }

        public void EliminarPermiso(int idUsuario, int permisoId)
        {
            permisoDAL.EliminarPermiso(idUsuario, permisoId);
        }

        public void InsertarPermisos(int idUsuario, IEnumerable<int> permisosIds)
        {
            permisoDAL.InsertarPermisos(idUsuario, permisosIds);
        }

        public int EliminarPermisos(int idUsuario)
        {
            return permisoDAL.EliminarPermisos(idUsuario);
        }
        public DataTable ObtenerPermisosDeCatalogo()
        {
            return permisoDAL.ObtenerPermisosDeCatalogo();
        }

        public DataTable ObtenerPermisosConcedidos(int usuarioId)
        {
            return permisoDAL.ObtenerPermisosConcedidos(usuarioId);
        }

    }
}
