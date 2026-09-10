using Microsoft.Data.SqlClient;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class PermisoDAL
    {
        private readonly string connectionString;
        public PermisoDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            HashSet<int> permisosIds = new HashSet<int>();
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpPermisosObtenerPorUsuarioId", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permisosIds.Add(reader.GetInt32(reader.GetOrdinal("PermisoId")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos concedidos del usuario: " + ex.Message);
            }
            return permisosIds;
        }

    }
}
