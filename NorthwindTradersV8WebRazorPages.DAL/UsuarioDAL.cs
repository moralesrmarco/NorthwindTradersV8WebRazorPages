using Microsoft.Data.SqlClient;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class UsuarioDAL
    {
        private readonly string connectionString;
        public UsuarioDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado)
        {
            int idUsuario = 0;
            nombreUsuarioAutenticado = string.Empty;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpUsuarioValidarLogin", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(
                        "@Usuario",
                        SqlDbType.VarChar,
                        20).Value = usuario;

                    cmd.Parameters.Add(
                        "@Password",
                        SqlDbType.VarChar,
                        64).Value = password;
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idUsuario = Convert.ToInt32(reader["Id"]);
                            string? paterno = reader["Paterno"].ToString();
                            string? materno = reader["Materno"].ToString();
                            string? nombres = reader["Nombres"].ToString();
                            nombreUsuarioAutenticado = $"{nombres} {paterno} {materno}";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el usuario: " + ex.Message);
            }
            return idUsuario;
        }
        public byte ValidarContraseñaActual(string usuario, string contrasenaActual)
        {
            byte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpUsuarioValidarContrasenaActual", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Password", contrasenaActual);
                    cn.Open();
                    numRegs = Convert.ToByte(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al validar la contraseña actual: " + ex.Message);
            }
            return numRegs;
        }
        public byte ActualizarContraseña(string usuario, string nuevaContrasena)
        {
            byte numRegs = 0;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpUsuarioActualizarContrasena", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@password", nuevaContrasena);
                    cn.Open();
                    numRegs = Convert.ToByte(cmd.ExecuteNonQuery());
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al cambiar la contraseña: " + ex.Message);
            }
            return numRegs;
        }

    }
}
