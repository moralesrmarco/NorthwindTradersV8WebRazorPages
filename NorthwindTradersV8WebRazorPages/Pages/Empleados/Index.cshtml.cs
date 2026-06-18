using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class IndexModel : PagedPageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        public DataTable Empleados { get; set; } = new DataTable();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            empleadoBLL = new EmpleadoBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Empleados = empleadoBLL.ObtenerEmpleadosPaginados(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
