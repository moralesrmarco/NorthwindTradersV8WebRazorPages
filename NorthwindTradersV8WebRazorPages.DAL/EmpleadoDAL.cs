using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class EmpleadoDAL
    {
        private readonly string connectionString;
        public EmpleadoDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable ObtenerEmpleadosPaginados(int pageIndex, int pageSize, out int totalRegistros)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpEmpleadosObtenerPaginados", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var adapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);

            // Primer resultset = total de registros
            totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);

            // Segundo resultset = empleados paginados
            return ds.Tables[1];
        }
        public byte[]? ObtenerEmpleadoFotoPorId(int employeeId)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpEmpleadoObtenerFotoPorId", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            connection.Open();
            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                return null;
            var fotoBytes = (byte[])result;
            return PhotoHelper.StripOleHeader(fotoBytes, employeeId);
        }

        public Empleado? ObtenerEmpleadoPorId(int id)
        {
            Empleado? empleado = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerPorId_WRP", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            empleado = MapearEmpleado(rdr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el empleado por id." + ex.Message);
            }
            return empleado;
        }

        private Empleado MapearEmpleado(SqlDataReader rdr)
        {
            var empleado = new Empleado();

            int ordEmployeeID = rdr.GetOrdinal("EmployeeID");
            int ordRowVersion = rdr.GetOrdinal("RowVersion");
            int ordFirstName = rdr.GetOrdinal("FirstName");
            int ordLastName = rdr.GetOrdinal("LastName");
            int ordTitle = rdr.GetOrdinal("Title");
            int ordTitleCourtesy = rdr.GetOrdinal("TitleOfCourtesy");
            int ordBirthDate = rdr.GetOrdinal("BirthDate");
            int ordHireDate = rdr.GetOrdinal("HireDate");
            int ordAddress = rdr.GetOrdinal("Address");
            int ordCity = rdr.GetOrdinal("City");
            int ordRegion = rdr.GetOrdinal("Region");
            int ordPostalCode = rdr.GetOrdinal("PostalCode");
            int ordCountry = rdr.GetOrdinal("Country");
            int ordHomePhone = rdr.GetOrdinal("HomePhone");
            int ordExtension = rdr.GetOrdinal("Extension");
            int ordNotes = rdr.GetOrdinal("Notes");
            int ordReportsTo = rdr.GetOrdinal("ReportsTo");
            int ordReportsToName = rdr.GetOrdinal("ReportsToName");

            empleado.EmployeeID = rdr.GetInt32(ordEmployeeID);
            empleado.RowVersion = rdr.IsDBNull(ordRowVersion) ? null : (byte[])rdr[ordRowVersion];

            empleado.FirstName = rdr.IsDBNull(ordFirstName) ? null : rdr.GetString(ordFirstName);
            empleado.LastName = rdr.IsDBNull(ordLastName) ? null : rdr.GetString(ordLastName);
            empleado.Title = rdr.IsDBNull(ordTitle) ? null : rdr.GetString(ordTitle);
            empleado.TitleOfCourtesy = rdr.IsDBNull(ordTitleCourtesy) ? null : rdr.GetString(ordTitleCourtesy);

            empleado.BirthDate = rdr.IsDBNull(ordBirthDate) ? (DateTime?)null : rdr.GetDateTime(ordBirthDate);
            empleado.HireDate = rdr.IsDBNull(ordHireDate) ? (DateTime?)null : rdr.GetDateTime(ordHireDate);

            empleado.Address = rdr.IsDBNull(ordAddress) ? null : rdr.GetString(ordAddress);
            empleado.City = rdr.IsDBNull(ordCity) ? null : rdr.GetString(ordCity);
            empleado.Region = rdr.IsDBNull(ordRegion) ? null : rdr.GetString(ordRegion);
            empleado.PostalCode = rdr.IsDBNull(ordPostalCode) ? null : rdr.GetString(ordPostalCode);
            empleado.Country = rdr.IsDBNull(ordCountry) ? null : rdr.GetString(ordCountry);
            empleado.HomePhone = rdr.IsDBNull(ordHomePhone) ? null : rdr.GetString(ordHomePhone);
            empleado.Extension = rdr.IsDBNull(ordExtension) ? null : rdr.GetString(ordExtension);
            empleado.Notes = rdr.IsDBNull(ordNotes) ? null : rdr.GetString(ordNotes);

            empleado.ReportsTo = rdr.IsDBNull(ordReportsTo) ? (int?)null : rdr.GetInt32(ordReportsTo);
            empleado.ReportsToName = rdr.IsDBNull(ordReportsToName) ? null : rdr.GetString(ordReportsToName);
            return empleado;
        }

    }
}
