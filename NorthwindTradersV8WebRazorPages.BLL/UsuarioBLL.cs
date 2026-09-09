using NorthwindTradersV8WebRazorPages.DAL;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL usuarioDAL;
        public UsuarioBLL(string connectionString)
        {
            this.usuarioDAL = new UsuarioDAL(connectionString);
        }
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado)
        {
            return usuarioDAL.ValidarUsuario(usuario, password, out nombreUsuarioAutenticado);
        }
    }
}
