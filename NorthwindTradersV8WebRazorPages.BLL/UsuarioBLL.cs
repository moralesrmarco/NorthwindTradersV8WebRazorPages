using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL usuarioDAL;
        public UsuarioBLL(string connectionString) => usuarioDAL = new UsuarioDAL(connectionString);
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado) => usuarioDAL.ValidarUsuario(usuario, password, out nombreUsuarioAutenticado);
        public byte ValidarContraseñaActual(string usuario, string contrasenaActual) => usuarioDAL.ValidarContraseñaActual(usuario, contrasenaActual);
        public byte ActualizarContraseña(string usuario, string nuevaContrasena) => usuarioDAL.ActualizarContraseña(usuario, nuevaContrasena);
        public DataTable Buscar(UsuariosBuscarDto filtro) => usuarioDAL.Buscar(filtro);
        public Usuario? ObtenerPorId(int id) => usuarioDAL.ObtenerPorId(id);
        public bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir = null) => usuarioDAL.ExisteNombreUsuario(nombreUsuario, idExcluir);
        public int Insertar(Usuario usuario) => usuarioDAL.Insertar(usuario);
        public int Actualizar(Usuario usuario) => usuarioDAL.Actualizar(usuario);
        public int Eliminar(Usuario usuario) => usuarioDAL.Eliminar(usuario);
    }
}
