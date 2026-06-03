using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL.Helpers
{
    public class ComboDataHelper
    {
        private readonly string connectionString;
        public ComboDataHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<ComboItemDto> LlenarCbo(string storedProcedure, params SqlParameter[] parameters)
        {
            var items = new List<ComboItemDto>();
            var dtTemp = new DataTable();
            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(storedProcedure, cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);
                    da.Fill(dtTemp);
                }
                // Insertar fila "Seleccione" al inicio
                items.Add(new ComboItemDto
                {
                    Value = "0",
                    Text = "»--- Seleccione ---«"
                });

                // Tomamos la primera y segunda columna sin importar el nombre
                foreach (DataRow row in dtTemp.Rows)
                {
                    items.Add(new ComboItemDto
                    {
                        Value = row[0]?.ToString() ?? string.Empty,
                        Text = row[1]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al llenar el ComboBox: " + ex.Message);
            }
            return items;
        }

    }
}
