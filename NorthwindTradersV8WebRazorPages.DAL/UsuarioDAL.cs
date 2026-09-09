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
    }
}
