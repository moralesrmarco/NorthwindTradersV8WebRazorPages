using NorthwindTradersV8WebRazorPages.DAL;

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
    }
}
