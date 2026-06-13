using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.DAL.Helpers;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
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
        public int Insertar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombres", empleado.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellidos", empleado.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Titulo", empleado.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TitCortesia", empleado.TitleOfCourtesy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FNacimiento", empleado.BirthDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FContratacion", empleado.HireDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", empleado.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", empleado.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoP", empleado.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pais",
                        string.IsNullOrWhiteSpace(empleado.Country) || empleado.Country == "0"
                            ? (object)DBNull.Value
                            : empleado.Country);
                    cmd.Parameters.AddWithValue("@Telefono", empleado.HomePhone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Extension", empleado.Extension ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notas", empleado.Notes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reportaa", string.IsNullOrWhiteSpace(empleado.ReportsTo.ToString()) || empleado.ReportsTo == -1
                        ? (object)DBNull.Value
                        : empleado.ReportsTo);
                    var pPhoto = cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1);
                    pPhoto.Value = empleado.Photo ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@Id", 0).Direction = ParameterDirection.Output;
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el empleado." + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", empleado.EmployeeID);
                    cmd.Parameters.AddWithValue("@Nombres", empleado.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellidos", empleado.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Titulo", empleado.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TitCortesia", empleado.TitleOfCourtesy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FNacimiento", empleado.BirthDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FContratacion", empleado.HireDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", empleado.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Region", empleado.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoP", empleado.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pais",
                        string.IsNullOrWhiteSpace(empleado.Country) || empleado.Country == "0"
                            ? (object)DBNull.Value
                            : empleado.Country);
                    cmd.Parameters.AddWithValue("@Telefono", empleado.HomePhone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Extension", empleado.Extension ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notas", empleado.Notes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reportaa", string.IsNullOrWhiteSpace(empleado.ReportsTo.ToString()) || empleado.ReportsTo == -1
                        ? (object)DBNull.Value
                        : empleado.ReportsTo);
                    var pPhoto = cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1);
                    pPhoto.Value = empleado.Photo ?? (object)DBNull.Value;
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = empleado.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el empleado." + ex.Message);
            }
            return numRegs;
        }
        public int Eliminar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", empleado.EmployeeID);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = empleado.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el empleado." + ex.Message);
            }
            return numRegs;
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
            byte[]? fotoBytes = null;
            if (result == null || result == DBNull.Value)
            {
                // Cargar la imagen por defecto desde wwwroot/images
                var defaultPath = Path.Combine("wwwroot", "images", "FotoPerfil.png");
                fotoBytes = File.ReadAllBytes(defaultPath);
            }
            else
                fotoBytes = (byte[])result;
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

        public EmpleadoRptDto? ObtenerEmpleadoPorIdRptDto(int id)
        {
            EmpleadoRptDto? empleado = null;
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
                            empleado = MapearEmpleadoRpt(rdr);
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
            empleado.Photo = ObtenerEmpleadoFotoPorId(empleado.EmployeeID);
            return empleado;
        }

        private EmpleadoRptDto? MapearEmpleadoRpt(SqlDataReader rdr)
        {
            var empleado = new EmpleadoRptDto();

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
            empleado.Photo = ObtenerEmpleadoFotoPorId(empleado.EmployeeID);
            return empleado;
        }
        public DataTable BuscarEmpleados(EmpleadosBuscarDto filtro)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SpEmpleadosBuscar", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
            cmd.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
            cmd.Parameters.AddWithValue("@Nombres", filtro.Nombres ?? string.Empty);
            cmd.Parameters.AddWithValue("@Apellidos", filtro.Apellidos ?? string.Empty);
            cmd.Parameters.AddWithValue("@Titulo", filtro.Titulo ?? string.Empty);
            cmd.Parameters.AddWithValue("@Domicilio", filtro.Domicilio ?? string.Empty);
            cmd.Parameters.AddWithValue("@Ciudad", filtro.Ciudad ?? string.Empty);
            cmd.Parameters.AddWithValue("@Region", filtro.Region ?? string.Empty);
            cmd.Parameters.AddWithValue("@CodigoP", filtro.CodigoP ?? string.Empty);
            cmd.Parameters.AddWithValue("@Pais", filtro.Pais ?? string.Empty);
            cmd.Parameters.AddWithValue("@Telefono", filtro.Telefono ?? string.Empty);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        public List<Empleado> ObtenerTodosLosEmpleados()
        {
            var empleados = new List<Empleado>();
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SpEmpleadoObtenerTodos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(MapearEmpleado(reader));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empleados;
        }
    }
}
